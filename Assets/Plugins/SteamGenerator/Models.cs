using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tarodev.SteamGenerator
{
    public class SteamStat
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("defaultvalue")] public int DefaultValue { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
    }

    public class SteamAchievement
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("defaultvalue")] public int DefaultValue { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("hidden")] public int Hidden { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("icongray")] public string IconGray { get; set; }
    }

    public class AvailableGameStats
    {
        [JsonProperty("achievements")] public List<SteamAchievement> Achievements { get; set; }
        [JsonProperty("stats")] public List<SteamStat> Stats { get; set; }
    }

    public class Game
    {
        [JsonProperty("gameName")] public string GameName { get; set; }
        [JsonProperty("gameVersion")] public string GameVersion { get; set; }
        [JsonProperty("availableGameStats")] public AvailableGameStats AvailableGameStats { get; set; }
    }

    public class GameSchema
    {
        [JsonProperty("game")] public Game Game { get; set; }
    }
}