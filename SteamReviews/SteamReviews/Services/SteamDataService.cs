using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SteamReviews.Models;

namespace SteamReviews.Services
{
	public static class SteamDataService
	{
		static readonly object FileLock = new object();

		public static GamePage LoadGamePage(string gameName)
		{
			var page = new GamePage { gameName = gameName, ReviewLangs = new List<ReviewLang>() };
			var folder = AppPaths.GameFolder(gameName);
			if (!Directory.Exists(folder))
				return page;

			foreach (var filePath in Directory.GetFiles(folder, "*.txt"))
			{
				var fileName = Path.GetFileName(filePath);
				var parts = fileName.Split('_');
				if (parts.Length < 2)
					continue;

				var rl = new ReviewLang
				{
					lang = parts[1].Split('.')[0],
					Links = File.ReadAllLines(filePath).ToList()
				};
				page.ReviewLangs.Add(rl);
			}
			return page;
		}

		public static GameStats ComputeStats(GamePage page, string steamAccount)
		{
			var stats = new GameStats();
			foreach (var rl in page.ReviewLangs)
			{
				foreach (var link in rl.Links)
				{
					stats.Collected++;
					if (link.Contains("?donsk"))
						stats.Spammed++;
					if (link.Contains("?closed"))
						stats.Closed++;
					if (link.Contains("?donskdel"))
						stats.Deleted++;
				}
			}

			var accountFile = AppPaths.AccountCommentsFile(steamAccount);
			if (File.Exists(accountFile))
			{
				foreach (var line in File.ReadAllLines(accountFile))
				{
					if (line.Contains("?del"))
						stats.Deleted++;
				}
			}
			return stats;
		}

		public static void SaveLanguageLinks(string gameName, string lang, List<string> links)
		{
			lock (FileLock)
			{
				var folder = AppPaths.GameFolder(gameName);
				Directory.CreateDirectory(folder);
				var path = Path.Combine(folder, gameName + "_" + lang + ".txt");
				File.WriteAllLines(path, links);
			}
		}

		public static void SaveAccountComments(string steamAccount, IEnumerable<string> links)
		{
			lock (FileLock)
			{
				File.WriteAllLines(AppPaths.AccountCommentsFile(steamAccount), links);
			}
		}

		public static string[] ReadAccountComments(string steamAccount)
		{
			var path = AppPaths.AccountCommentsFile(steamAccount);
			return File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
		}
	}
}
