#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.AutoSave
{
    public class AutoSaveSettingsProvider : MonoBehaviour
    {
         private const string SETTINGS_PATH = "Project/Tools/AutoSave";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider(SETTINGS_PATH, SettingsScope.Project)
            {
                label = "Auto Save",
                activateHandler = (_, rootElement) =>
                {
                    rootElement.styleSheets.Add(Resources.Load<StyleSheet>("AutoSaveStyle"));
                    var container = new VisualElement();
                    container.AddToClassList(Styles.Container);
                    rootElement.Add(container);
                    
                    container.Add(new Label("Auto Save Settings"));
                    
                    var configBox = new VisualElement();
                    configBox.AddToClassList(Styles.ConfigBox);
                    container.Add(configBox);
                    
                    var config = AutoSaveScriptable.instance.Config;
                    configBox.Add(CreateFieldRow("Enabled", CreateField<bool, Toggle>(config.Enabled, s => config.Enabled = s)));
                    configBox.Add(CreateFieldRow("Frequency (minutes)", CreateField<int, IntegerField>(config.Frequency, s => config.Frequency = s)));
                    configBox.Add(CreateFieldRow("Logging", CreateField<bool, Toggle>(config.Logging, s => config.Logging = s)));
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
                    AutoSaveScriptable.instance.Save();
                });
                return field;
            }
        }
        
        [MenuItem("Tools/AutoSave/Settings")]
        private static void OpenSettings() => SettingsService.OpenProjectSettings(SETTINGS_PATH);
    }
}

#endif