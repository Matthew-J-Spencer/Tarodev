using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Tarodev.FileWatcher
{
    [FilePath("File Watcher/FileWatcherConfig", FilePathAttribute.Location.PreferencesFolder)]
    public class FileWatcher : ScriptableSingleton<FileWatcher>
    {
        public Watcher SassConfig;
        public Watcher AnotherConfig;

        public void Save() => Save(true);
    }

    [Serializable]
    public class Watcher
    {
        public bool Enabled = true;
        public string Arguments;
    }

    internal static class FileWatcherSettingsProvider
    {
        private const string SETTINGS_PATH = "Project/FileWatcher";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "File Watcher",
                activateHandler = (_, rootElement) =>
                {
                    var config = FileWatcher.instance;
                    rootElement.Add(CreateConfigContainer("Sass", config.SassConfig));
                    rootElement.Add(CreateConfigContainer("Another", config.AnotherConfig));
                }
            };
            return provider;
        }

        private static VisualElement CreateConfigContainer(string title, Watcher settings)
        {
            var sassContainer = new VisualElement();
            sassContainer.Add(new Label(title));

            var row = new VisualElement { style = { flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row) } };

            row.Add(CreateField<bool, Toggle>(null, settings.Enabled, b => settings.Enabled = b));

            var argumentsField = CreateField<string, TextField>("Arguments", settings.Arguments, s => settings.Arguments = s);
            argumentsField.style.flexGrow = 1;
            row.Add(argumentsField);

            sassContainer.Add(row);
            return sassContainer;

            VisualElement CreateField<TValue, TField>(string label, TValue value, Action<TValue> onChange) where TField : BaseField<TValue>, new()
            {
                var field = new TField { label = label, value = value };
                field.RegisterValueChangedCallback(e =>
                {
                    onChange(e.newValue);
                    FileWatcher.instance.Save();
                });
                return field;
            }
        }

        [MenuItem("File Watcher/Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);
    }

    internal class FileWatcherProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var directory = Directory.GetCurrentDirectory();
            var settings = FileWatcher.instance;
            foreach (var asset in importedAssets)
            {
                var split = asset.Split('.');
                if (split.Length < 2) continue;

                var assetPath = $"{directory}/{split[0]}";
                if (settings.SassConfig.Enabled && split[1] is "scss" or "sass") ProcessSass(assetPath, split[1], settings.SassConfig.Arguments);
            }
        }

        private static void ProcessSass(string assetPath, string ext, string args) => RunProcess("sass", $"{assetPath}.{ext}:{assetPath}.uss --no-source-map {args}");

        private static void RunProcess(string programName, string argument)
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = programName,
                Arguments = argument
            };

            Process.Start(startInfo);
        }
    }
}