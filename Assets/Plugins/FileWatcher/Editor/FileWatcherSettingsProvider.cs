using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.FileWatcher
{
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
                    rootElement.styleSheets.Add(Resources.Load<StyleSheet>("FileWatcherStyle"));
                    var container = new VisualElement();
                    container.AddToClassList("container");
                    rootElement.Add(container);

                    container.Add(new Label("File Watcher Settings"));

                    if (!CheckInstall("npm"))
                    {
                        var errorContainer = new VisualElement();
                        errorContainer.Add(new Label("Node is not installed. <a href>Install</a>"));
                        errorContainer.RegisterCallback<MouseDownEvent>(_ => Application.OpenURL("https://nodejs.org/en/download/"));
                        container.Add(errorContainer);
                        return;
                    }

                    var watchers = FileWatcherScriptable.instance.Watchers;
                    foreach (var watcher in watchers)
                    {
                        container.Add(CreateWatcherContainer(watcher));
                    }
                }
            };
            return provider;
        }

        private static VisualElement CreateWatcherContainer(WatcherBase watcher)
        {
            var installed = CheckInstall(watcher.ProgramName);
            
            var configContainer = new VisualElement();
            configContainer.AddToClassList("config-container");

            var contentContainer = new VisualElement();
            contentContainer.AddToClassList("content-container");

            var titleRow = new VisualElement();
            titleRow.AddToClassList("title-row");
            var enabledToggle = CreateField<bool, Toggle>(watcher.Enabled && installed, b =>
            {
                watcher.Enabled = b;
                ToggleContentContainer(b);
            });
            titleRow.Add(enabledToggle);
            titleRow.Add(new Label(watcher.Name));

            configContainer.Add(titleRow);

            if (!installed)
            {
                var errorLabel = new Label($"{watcher.Name} is not installed. Run: {watcher.InstallCommand}");
                errorLabel.AddToClassList("error-label");
                titleRow.Add(errorLabel);
                
                enabledToggle.SetEnabled(false);
                if (watcher.Enabled)
                {
                    watcher.Enabled = false;
                    FileWatcherScriptable.instance.Save();
                }
                return configContainer;
            }

            configContainer.Add(contentContainer);

            contentContainer.Add(CreateFieldRow("Arguments",
                CreateField<string, TextField>(watcher.Arguments, s => watcher.Arguments = s)));

            contentContainer.Add(CreateFieldRow("Output Path",
                CreateField<string, TextField>(watcher.OutputPath, s => watcher.OutputPath = s),
                "Leave blank to use the source folder. Path is relative to project root (Example: GUI/Styles/Output)"));

            ToggleContentContainer(watcher.Enabled);
            return configContainer;

            void ToggleContentContainer(bool b) => contentContainer.style.display = b ? DisplayStyle.Flex : DisplayStyle.None;

            VisualElement CreateFieldRow(string label, VisualElement field, string tooltip = null)
            {
                var row = new VisualElement() { tooltip = tooltip };
                row.AddToClassList("field-row");
                row.Add(new Label(label));
                row.Add(field);
                return row;
            }

            VisualElement CreateField<TValue, TField>(TValue value, Action<TValue> onChange) where TField : BaseField<TValue>, new()
            {
                var field = new TField { value = value };

                field.RegisterValueChangedCallback(e =>
                {
                    onChange(e.newValue);
                    FileWatcherScriptable.instance.Save();
                });
                return field;
            }
        }

        // There's probably a better way to handle this
        private static bool CheckInstall(string program)
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = program
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [MenuItem("File Watcher/Open Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);
    }
}