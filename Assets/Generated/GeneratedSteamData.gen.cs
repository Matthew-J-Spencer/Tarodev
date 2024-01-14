using System.Collections.Generic;
using Tarodev.SteamGenerator;
// ReSharper disable InconsistentNaming

namespace Generated
{
	public static class GeneratedSteamData
	{
		public static readonly IReadOnlyDictionary<Achievement, SteamAchievement> Achievements = new Dictionary<Achievement, SteamAchievement>
		{
			{ Achievement.DEBUG_ACHIEVEMENT, new SteamAchievement { Name = "DEBUG_ACHIEVEMENT", DefaultValue = 0, DisplayName = "Debug Achievement", Hidden = 0, Description = "A debug achievement description", Icon = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/2630150/7679fc7673bc857473f88877ecb7ff18d1ed4475.jpg", IconGray = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/2630150/0655b197dce7a291a1d2957fb1a32b8138cab1ee.jpg" } },
		};

		public static readonly IReadOnlyDictionary<Stat, SteamStat> Stats = new Dictionary<Stat, SteamStat>
		{
			{ Stat.RUNS_COMPLETED, new SteamStat { Name = "RUNS_COMPLETED", DefaultValue = 0, DisplayName = "Runs Completed" } },
		};
	}

	public enum Achievement
	{
		DEBUG_ACHIEVEMENT,
	}

	public enum Stat
	{
		RUNS_COMPLETED,
	}
}