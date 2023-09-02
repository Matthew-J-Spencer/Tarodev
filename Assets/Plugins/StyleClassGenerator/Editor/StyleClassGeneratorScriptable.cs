using System;
using System.Collections.Generic;
using UnityEditor;

namespace Tarodev.StyleClassGenerator
{
    [FilePath("Tools/Style Class Generator.taro", FilePathAttribute.Location.ProjectFolder)]
    public class StyleClassGeneratorScriptable : ScriptableSingleton<StyleClassGeneratorScriptable>
    {
       
        public List<StyleClassGeneratorConfig> Configs = new() { new StyleClassGeneratorConfig() };
        public void Save() => Save(true);
    }

    [Serializable]
    public class StyleClassGeneratorConfig
    {
        public string Namespace;

        // public string Output;
        public string FileName;
        public string TargetDirectory;
        
        public bool AutoGenerate = true;
        public bool IncludeUnityClasses = false;
    }
}