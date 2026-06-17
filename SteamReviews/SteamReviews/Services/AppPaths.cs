using System;
using System.IO;

namespace SteamReviews.Services
{
	public static class AppPaths
	{
		public static string BaseDir => AppDomain.CurrentDomain.BaseDirectory;
		public static string SteamDir => Path.Combine(BaseDir, "Steam");
		public static string DataDir => Path.Combine(BaseDir, "Data");
		public static string GamesFile => Path.Combine(DataDir, "games.json");
		public static string PresetsDir => Path.Combine(DataDir, "Presets");

		public static void EnsureDirectories()
		{
			Directory.CreateDirectory(SteamDir);
			Directory.CreateDirectory(DataDir);
			Directory.CreateDirectory(PresetsDir);
		}

		public static string GameFolder(string gameName) => Path.Combine(SteamDir, gameName);

		public static string PresetFile(string presetId) => Path.Combine(PresetsDir, presetId + ".json");

		public static string AccountCommentsFile(string steamAccount) => Path.Combine(SteamDir, steamAccount + ".txt");
	}
}
