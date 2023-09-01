using System;
using System.Collections.Generic;
using UnityEditor;

namespace Tarodev.StyleClassGenerator
{
    public class StyleClassGeneratorScriptable : ScriptableSingleton<StyleClassGeneratorScriptable>
    {
        public bool AutoGenerate = true;
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
    }
}