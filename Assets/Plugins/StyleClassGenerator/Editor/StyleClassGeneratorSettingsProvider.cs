using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.StyleClassGenerator
{
    internal static class StyleClassGeneratorSettingsProvider
    {
        private const string SETTINGS_PATH = "Project/StyleClassGenerator";
        private static StyleClassGeneratorConfig Config => StyleClassGeneratorScriptable.instance.Config;
        private static readonly StyleClassGenerator Generator = new(StyleClassGeneratorScriptable.instance.Config);

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "Style Class Generator",
                activateHandler = (_, rootElement) =>
                {
                    rootElement.styleSheets.Add(Resources.Load<StyleSheet>("StyleClassGeneratorStyle"));
                    var container = new VisualElement();
                    container.AddToClassList("container");
                    rootElement.Add(container);

                    container.Add(new Label("Style Class Generator Settings"));

                    var saveBtn = new Button { text = "Save Changes" };
                    saveBtn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                    var autoGenerateToggle = CreateFieldRow<bool, Toggle>("Auto Generate", Config.AutoGenerate);
                    container.Add(autoGenerateToggle.row);

                    var outPutField = CreateFieldRow<string, TextField>("Output", Config.Output, "Leave blank to use root Assets/");
                    container.Add(outPutField.row);

                    var nameSpaceField = CreateFieldRow<string, TextField>("Namespace", Config.Namespace, "Leave blank for no namespace");
                    container.Add(nameSpaceField.row);

                    var fileNameField = CreateFieldRow<string, TextField>("File Name", Config.FileName);
                    container.Add(fileNameField.row);

                    container.Add(saveBtn);

                    saveBtn.clicked += () =>
                    {
                        try
                        {
                            var newOutput = SanitizeDirectoryPath(outPutField.field.value);
                            var newNamespace = Sanitize(nameSpaceField.field.value, '.');
                            var newFileName = Sanitize(fileNameField.field.value);

                            CleanUpIfPathChanged(newOutput, newFileName);

                            Config.Output = newOutput;
                            outPutField.field.value = newOutput;

                            Config.Namespace = newNamespace;
                            nameSpaceField.field.value = newNamespace;

                            Config.FileName = newFileName;
                            fileNameField.field.value = newFileName;

                            StyleClassGeneratorScriptable.instance.Save();
                            Generator.Generate();
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
                        row.AddToClassList("field-row");
                        row.Add(new Label(label));

                        var field = new TField { value = value };

                        field.RegisterValueChangedCallback(e => saveBtn.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex));

                        row.Add(field);
                        return (row, field);
                    }
                }
            };
            return provider;
        }

        private static void CleanUpIfPathChanged(string output, string fileName)
        {
            var currentPath = StyleClassGeneratorShared.GeneratePathAndFileName(Config.Output, Config.FileName);
            var newPath = StyleClassGeneratorShared.GeneratePathAndFileName(output, fileName);

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
            var sanitized = Regex.Replace(path, "[<>:\"|?* ]", "_");

            if (string.IsNullOrEmpty(sanitized)) return string.Empty;

            if (sanitized[0] == '_') sanitized = "_" + sanitized[1..];

            sanitized = sanitized.TrimEnd('_');

            return sanitized;
        }

        [MenuItem("Tools/Style Class Generator/Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);

        [MenuItem("Tools/Style Class Generator/Generate")]
        private static void ReGenerate() => Generator.Generate();
    }
}