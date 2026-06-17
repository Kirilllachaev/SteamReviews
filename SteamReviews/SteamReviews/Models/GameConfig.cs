using System;
using System.Collections.Generic;

namespace SteamReviews.Models
{
	public class GameConfig
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; } = "";
		public string StoreLink { get; set; } = "";

		public override string ToString() => Name;
	}

	public class GameStats
	{
		public int Collected { get; set; }
		public int Spammed { get; set; }
		public int Closed { get; set; }
		public int Deleted { get; set; }
	}

	public class ReviewLang
	{
		public string lang;
		public List<string> Links = new List<string>();
	}

	public class GamePage
	{
		public string gameName;
		public List<ReviewLang> ReviewLangs = new List<ReviewLang>();
	}
}
