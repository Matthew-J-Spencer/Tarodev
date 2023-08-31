using System;
using UnityEditor;

namespace Tarodev.StyleClassGenerator
{
    public class StyleClassGeneratorScriptable : ScriptableSingleton<StyleClassGeneratorScriptable>
    {
        public StyleClassGeneratorConfig Config;
        public void Save() => Save(true);
    }

    [Serializable]
    public class StyleClassGeneratorConfig
    {
        public bool AutoGenerate = true;
        public string Namespace;
        public string Output;
        public string FileName;
    }
}