using System;
using UnityEditor;

namespace Tarodev.SteamGenerator
{
    [FilePath("Tools/Steam Generator Config.taro", FilePathAttribute.Location.ProjectFolder)]
    public class SteamGeneratorScriptable : ScriptableSingleton<SteamGeneratorScriptable>
    {
        public SteamGeneratorConfig Config = new();
        public void Save() => Save(true);
    }

    [Serializable]
    public class SteamGeneratorConfig
    {
        public string AppID = "";
        public string APIKey = "";
        public string GeneratedOutputPath = "/Generated/";
        public string GeneratedFileName = "GeneratedSteamData";
        public string Namespace = "Generated";
        public string AchievementsEnumName = "Achievement";
        public string StatsEnumName = "Stat";
    }
}