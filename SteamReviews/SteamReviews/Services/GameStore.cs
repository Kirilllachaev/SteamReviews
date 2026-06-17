using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SteamReviews.Models;

namespace SteamReviews.Services
{
	public static class GameStore
	{
		static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

		public static List<GameConfig> Load()
		{
			AppPaths.EnsureDirectories();

			if (!File.Exists(AppPaths.GamesFile))
			{
				var migrated = MigrateFromSteamFolders();
				Save(migrated);
				return migrated;
			}

			try
			{
				var json = File.ReadAllText(AppPaths.GamesFile);
				var games = JsonSerializer.Deserialize<List<GameConfig>>(json) ?? new List<GameConfig>();
				if (games.Count == 0)
					games = MigrateFromSteamFolders();
				return games;
			}
			catch
			{
				return MigrateFromSteamFolders();
			}
		}

		public static void Save(List<GameConfig> games)
		{
			AppPaths.EnsureDirectories();
			var json = JsonSerializer.Serialize(games, JsonOptions);
			File.WriteAllText(AppPaths.GamesFile, json);
		}

		static List<GameConfig> MigrateFromSteamFolders()
		{
			var games = new List<GameConfig>();
			if (!Directory.Exists(AppPaths.SteamDir))
				return games;

			foreach (var dir in Directory.GetDirectories(AppPaths.SteamDir))
			{
				var name = Path.GetFileName(dir);
				if (string.IsNullOrEmpty(name) || name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
					continue;
				if (File.Exists(dir))
					continue;

				games.Add(new GameConfig { Name = name, StoreLink = "" });
			}
			return games;
		}

		public static GameConfig Add(string name, string storeLink)
		{
			var game = new GameConfig { Name = name.Trim(), StoreLink = storeLink.Trim() };
			Directory.CreateDirectory(AppPaths.GameFolder(game.Name));
			return game;
		}

		public static void Delete(GameConfig game, List<GameConfig> games)
		{
			games.RemoveAll(g => g.Id == game.Id);
			var folder = AppPaths.GameFolder(game.Name);
			if (Directory.Exists(folder))
				Directory.Delete(folder, true);
		}
	}
}
