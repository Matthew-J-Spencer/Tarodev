using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.StyleClassGenerator
{
    internal static class StyleClassGeneratorSettingsProvider
    {
        private const string SETTINGS_PATH = "Project/Tools/StyleClassGenerator";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "Style Class Generator",
                activateHandler = (_, rootElement) => { CreateInterface(rootElement); }
            };
            return provider;
        }

        private static void CreateInterface(VisualElement rootElement)
        {
            rootElement.Clear();
            rootElement.styleSheets.Add(Resources.Load<StyleSheet>("StyleClassGeneratorStyle"));

            var settings = StyleClassGeneratorScriptable.instance;
            var configs = settings.Configs;

            // var autoGenerateToggle = new Toggle(){value = settings.AutoGenerate,label = "Auto Generate"};
            // autoGenerateToggle.RegisterValueChangedCallback(e =>
            // {
            //     settings.AutoGenerate = e.newValue;
            //     StyleClassGeneratorScriptable.instance.Save();
            // });
            //
            // rootElement.Add(autoGenerateToggle);

            var container = new VisualElement();
            container.AddToClassList(Styles.Container);
            rootElement.Add(container);

            container.Add(new Label("Style Class Generator Settings"));

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            container.Add(scroll);

            foreach (var config in configs)
            {
                var configBox = new VisualElement();
                configBox.AddToClassList(Styles.ConfigBox);
                scroll.Add(configBox);

                var saveBtn = new Button { text = "Save Changes" };
                saveBtn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                var targetDirectoryField = CreateFieldRow<string, TextField>("Target Directory", config.TargetDirectory, "Leave blank to use root Assets/");
                configBox.Add(targetDirectoryField.row);

                var nameSpaceField = CreateFieldRow<string, TextField>("Namespace", config.Namespace, "Leave blank for no namespace");
                configBox.Add(nameSpaceField.row);

                var fileNameField = CreateFieldRow<string, TextField>("File Name", config.FileName);
                configBox.Add(fileNameField.row);

                var autoGenerateField = CreateFieldRow<bool, Toggle>("Autogenerate", config.AutoGenerate, "If true, the file will be generated automatically when a .uss file is altered");
                configBox.Add(autoGenerateField.row);

                var includeUnityClassesField = CreateFieldRow<bool, Toggle>("Include Unity Classes", config.IncludeUnityClasses, "Exclude built-in classes which start with 'unity-'");
                configBox.Add(includeUnityClassesField.row);

                configBox.Add(saveBtn);

                saveBtn.clicked += () =>
                {
                    try
                    {
                        var newTargetDirectory = SanitizeDirectoryPath(targetDirectoryField.field.value);
                        var newNamespace = Sanitize(nameSpaceField.field.value, '.');
                        var newFileName = Sanitize(fileNameField.field.value);

                        CleanUpIfPathChanged(config, newTargetDirectory, newFileName);

                        config.TargetDirectory = newTargetDirectory;
                        targetDirectoryField.field.value = newTargetDirectory;

                        config.Namespace = newNamespace;
                        nameSpaceField.field.value = newNamespace;

                        config.FileName = newFileName;
                        fileNameField.field.value = newFileName;
                        
                        config.AutoGenerate = autoGenerateField.field.value;
                        autoGenerateField.field.value = config.AutoGenerate;

                        StyleClassGeneratorScriptable.instance.Save();
                        ReGenerate();
                        saveBtn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                };

                (VisualElement row, TField field) CreateFieldRow<TValue, TField>(string label, TValue value, string tooltip = null) where TField : BaseField<TValue>, new()
                {
                    var row = new VisualElement() { tooltip = tooltip };
                    row.AddToClassList(Styles.FieldRow);
                    row.Add(new Label(label));

                    var field = new TField { value = value };

                    field.RegisterValueChangedCallback(e => saveBtn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex));

                    row.Add(field);
                    return (row, field);
                }
            }

            var filler = new VisualElement();
            filler.AddToClassList(Styles.Filler);
            container.Add(filler);

            var btnRow = new VisualElement();
            btnRow.AddToClassList(Styles.BtnRow);
            container.Add(btnRow);

            var newRowButton = new Button(() =>
            {
                configs.Add(new StyleClassGeneratorConfig());
                StyleClassGeneratorScriptable.instance.Save();
                CreateInterface(rootElement);
            }) { text = "Add new configuration" };
            btnRow.Add(newRowButton);

            if (configs.Count > 1)
            {
                var removeRowButton = new Button(() =>
                {
                    configs.Remove(configs.Last());
                    StyleClassGeneratorScriptable.instance.Save();
                    CreateInterface(rootElement);
                }) { text = "Remove last configuration" };
                btnRow.Add(removeRowButton);
            }

            var generateButton = new Button(ReGenerate) { text = "Generate" };
            container.Add(generateButton);
        }

        private static void CleanUpIfPathChanged(StyleClassGeneratorConfig config, string targetDirectory, string fileName)
        {
            var currentPath = StyleClassGeneratorShared.GeneratePathAndFileName(config.TargetDirectory, config.FileName);
            var newPath = StyleClassGeneratorShared.GeneratePathAndFileName(targetDirectory, fileName);

            if (currentPath != newPath && File.Exists(currentPath))
            {
                File.Delete(currentPath);
            }
        }

        private static string Sanitize(string name, params char[] additionalAllowedChars)
        {
            var allowedCharsPattern = string.Join("", Array.ConvertAll(additionalAllowedChars, c => Regex.Escape(c.ToString())));
            var pattern = $"[^a-zA-Z0-9{allowedCharsPattern}]";

            var sanitized = Regex.Replace(name, pattern, "");
            if (sanitized.Length == 0) return string.Empty;

            if (sanitized.Length == 0 || !char.IsLetter(sanitized[0])) sanitized = $"G{sanitized}";

            return sanitized;
        }

        private static string SanitizeDirectoryPath(string path)
        {
            var sanitized = Regex.Replace(path, "[<>:\"|?*]", "_");

            if (string.IsNullOrEmpty(sanitized)) return string.Empty;

            if (sanitized[0] == '_') sanitized = "_" + sanitized[1..];

            sanitized = sanitized.TrimEnd('_');

            return sanitized;
        }

        [MenuItem("Tools/Style Class Generator/Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);

        [MenuItem("Tools/Style Class Generator/Generate")]
        private static void ReGenerate() => new StyleClassGenerator(StyleClassGeneratorScriptable.instance.Configs).Generate(isAutoGenerating: false);
    }
}