using System;

namespace SteamReviews.Models
{
	public class ProcessPreset
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; } = "Новый пресет";
		public string GameId { get; set; } = "";
		public string PromoGameId { get; set; } = "";
		public string SteamAccount { get; set; } = "";
		public string ProxyHost { get; set; } = "146.247.113.34";
		public string ProxyPort { get; set; } = "59100";
		public string Proxy { get; set; } = "";
		public bool UseProxy { get; set; } = true;
		public string SelectedLanguage { get; set; } = "all";
		public bool ContinueCollection { get; set; } = true;

		public string GetProxyAddress()
		{
			var host = ProxyHost?.Trim();
			var port = ProxyPort?.Trim();
			if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
				return "";
			return host + ":" + port;
		}

		public void MigrateLegacyProxy()
		{
			if (!string.IsNullOrWhiteSpace(ProxyHost) && !string.IsNullOrWhiteSpace(ProxyPort))
				return;
			if (string.IsNullOrWhiteSpace(Proxy))
				return;

			var parts = Proxy.Split(':');
			if (parts.Length < 2)
				return;

			ProxyPort = parts[parts.Length - 1];
			ProxyHost = string.Join(":", parts, 0, parts.Length - 1);
		}
	}
}
