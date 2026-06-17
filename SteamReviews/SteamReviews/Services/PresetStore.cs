using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SteamReviews.Models;

namespace SteamReviews.Services
{
	public static class PresetStore
	{
		static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

		public static List<ProcessPreset> LoadAll()
		{
			AppPaths.EnsureDirectories();
			var presets = new List<ProcessPreset>();

			if (!Directory.Exists(AppPaths.PresetsDir))
				return presets;

			foreach (var file in Directory.GetFiles(AppPaths.PresetsDir, "*.json"))
			{
				try
				{
					var json = File.ReadAllText(file);
					var preset = JsonSerializer.Deserialize<ProcessPreset>(json);
					if (preset != null)
					{
						preset.MigrateLegacyProxy();
						presets.Add(preset);
					}
				}
				catch { }
			}
			return presets;
		}

		public static void Save(ProcessPreset preset)
		{
			AppPaths.EnsureDirectories();
			var json = JsonSerializer.Serialize(preset, JsonOptions);
			File.WriteAllText(AppPaths.PresetFile(preset.Id), json);
		}

		public static void Delete(ProcessPreset preset)
		{
			var path = AppPaths.PresetFile(preset.Id);
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
