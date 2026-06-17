using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SteamReviews.Models;
using SteamReviews.Services;

namespace SteamReviews.Workers
{
	public class SteamWorker : IDisposable
	{
		readonly ProcessPreset _preset;
		readonly Func<GameConfig> _getGame;
		readonly Func<GameConfig> _getPromoGame;
		readonly Action<Action> _invoke;

		ChromeDriver _driver;
		Thread _workThread;
		volatile bool _isScrolling;
		volatile bool _isCollecting;
		volatile bool _stopRequested;

		GamePage _gamePage;
		List<string> _allLinks = new List<string>();

		public event Action<string> StatusChanged;
		public event Action<GameStats> StatsChanged;
		public event Action StateChanged;

		public bool IsChromeRunning => _driver != null;
		public bool IsBusy => _workThread != null && _workThread.IsAlive;

		public SteamWorker(ProcessPreset preset, Func<GameConfig> getGame, Func<GameConfig> getPromoGame, Action<Action> invoke)
		{
			_preset = preset;
			_getGame = getGame;
			_getPromoGame = getPromoGame;
			_invoke = invoke;
		}

		public void ReloadData()
		{
			var game = _getGame();
			if (game == null || string.IsNullOrEmpty(game.Name))
			{
				_gamePage = new GamePage();
				NotifyStats();
				return;
			}
			_gamePage = SteamDataService.LoadGamePage(game.Name);
			NotifyStats();
		}

		public void StartChrome()
		{
			var game = _getGame();
			if (game == null || string.IsNullOrEmpty(game.Name))
			{
				SetStatus("Выберите целевую игру");
				return;
			}
			if (string.IsNullOrEmpty(game.StoreLink))
			{
				SetStatus("У целевой игры не указана ссылка");
				return;
			}

			Stop(silent: true);
			_stopRequested = false;

			try
			{
				var usingProxy = _preset.UseProxy && !string.IsNullOrWhiteSpace(_preset.GetProxyAddress());
				SetStatus(usingProxy
					? "Запуск Chrome через прокси " + _preset.GetProxyAddress() + "..."
					: "Запуск Chrome...");

				if (usingProxy)
				{
					var options = new ChromeOptions();
					var proxyAddress = _preset.GetProxyAddress();
					var proxy = new Proxy
					{
						Kind = ProxyKind.Manual,
						HttpProxy = proxyAddress,
						SslProxy = proxyAddress
					};
					options.Proxy = proxy;
					_driver = new ChromeDriver(options);
				}
				else
				{
					_driver = new ChromeDriver();
				}

				SetStatus(usingProxy
					? "Chrome запущен, подключение через прокси к Steam..."
					: "Chrome запущен, открываю Steam...");

				_driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");
				ReloadData();
				SetStatus(usingProxy
					? "Войдите в Steam вручную (прокси: " + _preset.GetProxyAddress() + ")"
					: "Войдите в Steam вручную");
				NotifyState();
			}
			catch (Exception ex)
			{
				SetStatus("Ошибка запуска Chrome:\r\n" + DescribeError(ex) + GetErrorHint(ex, _preset));
				CloseChrome();
				NotifyState();
			}
		}

		public void Stop(bool silent = false)
		{
			_stopRequested = true;
			_isScrolling = false;
			_isCollecting = false;

			if (IsBusy && !silent)
				SetStatus("Останавливается...");

			CloseChrome();

			if (!silent)
				SetStatus("Выключен");

			NotifyStats();
			NotifyState();
		}

		public void StartCollect()
		{
			if (!EnsureReady())
				return;

			_stopRequested = false;
			_isCollecting = true;
			NotifyState();
			_workThread = new Thread(CollectWorker) { IsBackground = true };
			_workThread.Start();
		}

		public void StartSpam()
		{
			if (!EnsureReady())
				return;

			_stopRequested = false;
			NotifyState();
			_workThread = new Thread(SpamWorker) { IsBackground = true };
			_workThread.Start();
		}

		public void StartDelete()
		{
			if (!EnsureReady())
				return;

			if (string.IsNullOrWhiteSpace(_preset.SteamAccount))
			{
				SetStatus("Укажите ник аккаунта для удаления");
				return;
			}

			_stopRequested = false;
			NotifyState();
			_workThread = new Thread(DeleteWorker) { IsBackground = true };
			_workThread.Start();
		}

		bool EnsureReady()
		{
			if (_driver == null)
			{
				SetStatus("Сначала нажмите «Запустить»");
				return false;
			}
			if (IsBusy)
			{
				SetStatus("Сначала нажмите «Выключить» или дождитесь завершения");
				return false;
			}
			return true;
		}

		void CollectWorker()
		{
			try
			{
				var game = _getGame();
				ReloadData();

				if (_preset.SelectedLanguage == "all")
				{
					for (int i = 1; i < SteamLanguages.All.Length; i++)
					{
						if (!_isCollecting || _stopRequested)
							break;

						var lang = SteamLanguages.All[i];
						if (_preset.ContinueCollection && _gamePage.ReviewLangs.Any(r => r.lang == lang))
						{
							SetStatus("Сбор [" + lang + "]: уже собран, пропуск");
							continue;
						}

						CollectLanguage(game, lang);
					}

					if (!_stopRequested)
						SetStatus(IsChromeRunning ? "Сбор завершён — готов к работе" : "Сбор завершён");
				}
				else
				{
					CollectLanguage(game, _preset.SelectedLanguage);
					if (!_stopRequested)
						SetStatus(IsChromeRunning ? "Сбор [" + _preset.SelectedLanguage + "] завершён" : "Сбор завершён");
				}
			}
			catch (Exception ex)
			{
				SetStatus("Ошибка сбора:\r\n" + DescribeError(ex));
			}
			finally
			{
				_isCollecting = false;
				_isScrolling = false;
				if (_stopRequested)
					SetStatus("Сбор остановлен");
				NotifyStats();
				NotifyState();
			}
		}

		void CollectLanguage(GameConfig game, string lang)
		{
			SetStatus("Сбор [" + lang + "]: открываю страницу отзывов...");
			_allLinks.Clear();

			_driver.Navigate().GoToUrl(game.StoreLink + "/?p=1&browsefilter=all&filterLanguage=" + lang);
			Thread.Sleep(1000);
			TryDismissAgeGate();

			SetStatus("Сбор [" + lang + "]: прокрутка отзывов...");
			_isScrolling = true;
			ScrollUntilEnd(_driver, lang);

			if (_stopRequested)
				return;

			if (_allLinks.Count > 0)
			{
				SteamDataService.SaveLanguageLinks(game.Name, lang, _allLinks);
				ReloadData();
				SetStatus("Сбор [" + lang + "]: сохранено " + _allLinks.Count + " ссылок");
			}
			else
			{
				SetStatus("Сбор [" + lang + "]: ссылки не найдены");
			}

			_allLinks.Clear();
		}

		void ScrollUntilEnd(ChromeDriver driver, string lang)
		{
			var scrollPass = 0;
			while (_isScrolling && !_stopRequested)
			{
				scrollPass++;
				((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
				SetStatus("Сбор [" + lang + "]: прокрутка, найдено " + _allLinks.Count);
				Thread.Sleep(100);

				try
				{
					var end = driver.FindElement(By.ClassName("apphub_NoMoreContent"));
					if (end.GetAttribute("style").Contains("opacity: 1;"))
					{
						var cards = driver.FindElements(By.ClassName("apphub_Card"));
						foreach (var card in cards)
						{
							var link = card.GetAttribute("data-modal-content-url");
							if (!string.IsNullOrEmpty(link) && !_allLinks.Contains(link))
								_allLinks.Add(link);
						}
						_isScrolling = false;
						return;
					}
				}
				catch { }

				Thread.Sleep(1000);
			}
		}

		void SpamWorker()
		{
			try
			{
				var game = _getGame();
				ReloadData();

				if (_gamePage.ReviewLangs.Count == 0)
				{
					SetStatus("Спам: нет собранных отзывов");
					return;
				}

				var promoGame = _getPromoGame();
				if (promoGame == null || string.IsNullOrEmpty(promoGame.Name))
				{
					SetStatus("Спам: выберите промо-игру");
					return;
				}
				if (string.IsNullOrEmpty(promoGame.StoreLink))
				{
					SetStatus("Спам: у промо-игры нет ссылки");
					return;
				}

				var pending = CountPendingSpam();
				if (pending == 0)
				{
					SetStatus("Спам: все ссылки уже обработаны");
					return;
				}

				var processed = 0;
				foreach (var rl in _gamePage.ReviewLangs)
				{
					if (_stopRequested)
						break;

					if (!SteamMessages.Texts.ContainsKey(rl.lang))
						continue;

					for (int i = 0; i < rl.Links.Count; i++)
					{
						if (_stopRequested)
							break;

						var link = rl.Links[i];
						if (link.Contains("?donsk") || link.Contains("?closed"))
							continue;

						processed++;
						SetStatus("Спам [" + rl.lang + "]: " + processed + "/" + pending);
						_driver.Navigate().GoToUrl(link);
						Thread.Sleep(1000);

						try
						{
							var textarea = _driver.FindElement(By.ClassName("commentthread_textarea"));
							var text = SteamMessages.Texts[rl.lang];
							text += promoGame.Name;
							text += "\n";
							text += promoGame.StoreLink;
							textarea.SendKeys(text);
							Thread.Sleep(1000);

							var greenButton = _driver.FindElement(By.ClassName("btn_green_white_innerfade"));
							greenButton.Click();
							Thread.Sleep(500);

							try
							{
								var errorElement = _driver.FindElement(By.ClassName("commentthread_entry_error"));
								if (errorElement.Text.Contains("слишком часто"))
								{
									SetStatus("Спам остановлен: мут аккаунта или прокси");
									return;
								}
							}
							catch { }

							rl.Links[i] += "?donsk";
						}
						catch
						{
							rl.Links[i] += "?closed";
						}

						SteamDataService.SaveLanguageLinks(game.Name, rl.lang, rl.Links);
						NotifyStats();
					}
				}

				if (!_stopRequested)
					SetStatus(IsChromeRunning ? "Спам завершён — готов к работе" : "Спам завершён");
			}
			catch (Exception ex)
			{
				SetStatus("Ошибка спама:\r\n" + DescribeError(ex));
			}
			finally
			{
				if (_stopRequested)
					SetStatus("Спам остановлен");
				NotifyStats();
				NotifyState();
			}
		}

		void DeleteWorker()
		{
			try
			{
				var game = _getGame();
				ReloadData();

				var pending = CountPendingDelete();
				if (pending == 0)
				{
					SetStatus("Удаление: нет комментариев для удаления");
					return;
				}

				var processed = 0;
				foreach (var rl in _gamePage.ReviewLangs)
				{
					if (_stopRequested)
						break;

					for (int i = 0; i < rl.Links.Count; i++)
					{
						if (_stopRequested)
							break;

						var link = rl.Links[i];
						if (!link.Contains("?donsk") || link.Contains("?donskdel"))
							continue;

						processed++;
						SetStatus("Удаление [" + rl.lang + "]: " + processed + "/" + pending);
						_driver.Navigate().GoToUrl(link);
						Thread.Sleep(1000);

						try
						{
							var elements = _driver.FindElements(By.ClassName("commentthread_comment"));
							var actions = new Actions(_driver);

							foreach (var element1 in elements)
							{
								actions.MoveToElement(element1).Perform();
								Thread.Sleep(500);

								var bdiElement = element1.FindElement(By.TagName("bdi"));
								if (bdiElement.Text == _preset.SteamAccount)
								{
									var deleteBtn = _driver.FindElement(By.CssSelector("[data-tooltip-text='Удалить']"));
									deleteBtn.Click();
									Thread.Sleep(100);
									break;
								}
							}

							rl.Links[i] += "?donskdel";
						}
						catch { }

						SteamDataService.SaveLanguageLinks(game.Name, rl.lang, rl.Links);
						NotifyStats();
					}
				}

				if (!_stopRequested)
					SetStatus(IsChromeRunning ? "Удаление завершено — готов к работе" : "Удаление завершено");
			}
			catch (Exception ex)
			{
				SetStatus("Ошибка удаления:\r\n" + DescribeError(ex));
			}
			finally
			{
				if (_stopRequested)
					SetStatus("Удаление остановлено");
				NotifyStats();
				NotifyState();
			}
		}

		int CountPendingSpam()
		{
			var count = 0;
			foreach (var rl in _gamePage.ReviewLangs)
			{
				foreach (var link in rl.Links)
				{
					if (!link.Contains("?donsk") && !link.Contains("?closed"))
						count++;
				}
			}
			return count;
		}

		int CountPendingDelete()
		{
			var count = 0;
			foreach (var rl in _gamePage.ReviewLangs)
			{
				foreach (var link in rl.Links)
				{
					if (link.Contains("?donsk") && !link.Contains("?donskdel"))
						count++;
				}
			}
			return count;
		}

		void TryDismissAgeGate()
		{
			try
			{
				Thread.Sleep(500);
				_driver.FindElement(By.Id("ViewAllForApp")).Click();
				Thread.Sleep(500);
				_driver.FindElement(By.Id("age_gate_btn_continue")).Click();
				Thread.Sleep(500);
			}
			catch { }
		}

		void CloseChrome()
		{
			try
			{
				_driver?.Quit();
			}
			catch { }
			_driver = null;
		}

		void SetStatus(string text) => _invoke(() => StatusChanged?.Invoke(text));

		static string DescribeError(Exception ex)
		{
			var sb = new StringBuilder();
			for (var e = ex; e != null; e = e.InnerException)
			{
				if (sb.Length > 0)
					sb.AppendLine();
				sb.Append(e.Message);
			}
			sb.AppendLine();
			sb.Append('[').Append(ex.GetType().Name).Append(']');
			return sb.ToString();
		}

		static string GetErrorHint(Exception ex, ProcessPreset preset)
		{
			var text = DescribeError(ex);
			if (text.Contains("ERR_TUNNEL_CONNECTION_FAILED", StringComparison.OrdinalIgnoreCase) ||
			    text.Contains("ERR_PROXY", StringComparison.OrdinalIgnoreCase) ||
			    text.Contains("ERR_CONNECTION", StringComparison.OrdinalIgnoreCase))
			{
				return "\r\n\r\nЭто ошибка прокси, не ChromeDriver. Chrome уже запустился, но не смог подключиться через прокси.\r\n" +
				       "• Проверьте адрес и порт (нужен HTTP-прокси, не SOCKS5)\r\n" +
				       "• Убедитесь, что прокси онлайн и не требует логин/пароль (сейчас не поддерживается)\r\n" +
				       "• Для второго процесса — другой прокси или отключите прокси на одном из пресетов\r\n" +
				       "• Один прокси часто не держит 2 Chrome одновременно";
			}

			if (text.Contains("chromedriver", StringComparison.OrdinalIgnoreCase) ||
			    text.Contains("driver executable", StringComparison.OrdinalIgnoreCase))
			{
				return "\r\n\r\nПроверьте: установлен ли Google Chrome, пересоберите проект (нужен chromedriver.exe рядом с программой).";
			}

			if (preset.UseProxy)
				return "\r\n\r\nПрокси включён: " + preset.GetProxyAddress();

			return "";
		}

		void NotifyStats()
		{
			var game = _getGame();
			if (game == null)
				return;
			_gamePage = SteamDataService.LoadGamePage(game.Name);
			var stats = SteamDataService.ComputeStats(_gamePage, _preset.SteamAccount);
			_invoke(() => StatsChanged?.Invoke(stats));
		}

		void NotifyState() => _invoke(() => StateChanged?.Invoke());

		public void Dispose()
		{
			Stop(silent: true);
		}
	}
}
