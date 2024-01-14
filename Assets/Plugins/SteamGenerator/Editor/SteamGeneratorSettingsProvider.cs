using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.SteamGenerator
{
    internal static class SteamGeneratorSettingsProvider
    {
        private const string SETTINGS_PATH = "Project/Tools/SteamGenerator";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "Steam Generator",
                activateHandler = (_, rootElement) =>
                {
                    rootElement.styleSheets.Add(Resources.Load<StyleSheet>("SteamGeneratorStyle"));
                    var container = new VisualElement();
                    container.AddToClassList(Styles.Container);
                    rootElement.Add(container);
                    
                    container.Add(new Label("Steam Generator Settings"));
                    
                    var configBox = new VisualElement();
                    configBox.AddToClassList(Styles.ConfigBox);
                    container.Add(configBox);
                    
                    var config = SteamGeneratorScriptable.instance.Config;
                    configBox.Add(CreateFieldRow("App ID", CreateField<string, TextField>(config.AppID, s => config.AppID = s)));
                    configBox.Add(CreateFieldRow("API Key", CreateField<string, TextField>(config.APIKey, s => config.APIKey = s)));
                    configBox.Add(CreateFieldRow("Output Path", CreateField<string, TextField>(config.GeneratedOutputPath, s => config.GeneratedOutputPath = s)));
                    configBox.Add(CreateFieldRow("Output File Name", CreateField<string, TextField>(config.GeneratedFileName, s => config.GeneratedFileName = s)));
                    configBox.Add(CreateFieldRow("Namespace", CreateField<string, TextField>(config.Namespace, s => config.Namespace = s)));
                    configBox.Add(CreateFieldRow("Achievement Enum Name", CreateField<string, TextField>(config.AchievementsEnumName, s => config.AchievementsEnumName = s)));
                    configBox.Add(CreateFieldRow("Stat Enum Name", CreateField<string, TextField>(config.StatsEnumName, s => config.StatsEnumName = s)));
                }
            };
            return provider;
            
            VisualElement CreateFieldRow(string label, VisualElement field, string tooltip = null)
            {
                var row = new VisualElement() { tooltip = tooltip };
                row.AddToClassList(Styles.FieldRow);
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
                    SteamGeneratorScriptable.instance.Save();
                });
                return field;
            }
        }
        
        [MenuItem("Tools/Steam Generator/Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);
    }
}