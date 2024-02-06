#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Tarodev.AutoSave
{
    [FilePath("Tools/Auto Save Config.taro", FilePathAttribute.Location.ProjectFolder)]
    public class AutoSaveScriptable : ScriptableSingleton<AutoSaveScriptable>
    {
        public AutoSaveConfig Config = new();
        public void Save() => Save(true);
    }

    [Serializable]
    public class AutoSaveConfig
    {
        public bool Enabled = true;
        public int Frequency = 5;
        public bool Logging;
    }
}

#endif