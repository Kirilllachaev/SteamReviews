using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SteamReviews.Models;
using SteamReviews.Services;
using SteamReviews.Workers;

namespace SteamReviews.Controls
{
	public class ProcessPresetControl : UserControl
	{
		const int ControlWidth = 860;
		const int LabelX = 10;
		const int FieldX = 90;
		const int FieldWidth = ControlWidth - FieldX - 20;

		readonly ProcessPreset _preset;
		readonly Func<System.Collections.Generic.List<GameConfig>> _getGames;
		readonly Action<ProcessPreset> _onChanged;
		readonly Action<ProcessPreset> _onDelete;

		SteamWorker _worker;
		Timer _statsTimer;
		bool _suppressEvents;
		Font _uiFont;

		TextBox _nameBox;
		ComboBox _gameCombo;
		ComboBox _promoGameCombo;
		TextBox _accountBox;
		TextBox _proxyHostBox;
		TextBox _proxyPortBox;
		CheckBox _useProxyCheck;
		ComboBox _langCombo;
		CheckBox _continueCheck;
		Label _lblCollected;
		Label _lblSpammed;
		Label _lblClosed;
		Label _lblDeleted;
		TextBox _statusBox;
		Button _btnCopyStatus;
		Button _btnStart;
		Button _btnStop;
		Button _btnCollect;
		Button _btnSpam;
		Button _btnDelete;

		public ProcessPresetControl(
			ProcessPreset preset,
			Func<System.Collections.Generic.List<GameConfig>> getGames,
			Action<ProcessPreset> onChanged,
			Action<ProcessPreset> onDelete)
		{
			_preset = preset;
			_preset.MigrateLegacyProxy();
			_getGames = getGames;
			_onChanged = onChanged;
			_onDelete = onDelete;
			_uiFont = new Font("Segoe UI", 10);

			_worker = new SteamWorker(_preset, GetSelectedGame, GetPromoGame, action =>
			{
				if (InvokeRequired)
					Invoke(action);
				else
					action();
			});
			_worker.StatusChanged += OnStatusChanged;
			_worker.StatsChanged += UpdateStats;
			_worker.StateChanged += UpdateButtonStates;

			_statsTimer = new Timer { Interval = 1500 };
			_statsTimer.Tick += (_, __) => _worker.ReloadData();
			_statsTimer.Start();

			Tag = preset.Id;
			BuildUi();
			RefreshGameList();
			_worker.ReloadData();
			UpdateButtonStates();
		}

		GameConfig GetSelectedGame() =>
			_getGames().FirstOrDefault(g => g.Id == _preset.GameId);

		GameConfig GetPromoGame() =>
			_getGames().FirstOrDefault(g => g.Id == _preset.PromoGameId);

		void BuildUi()
		{
			Size = new Size(ControlWidth, 430);
			BorderStyle = BorderStyle.FixedSingle;
			Margin = new Padding(6);
			Font = _uiFont;

			int y = 10;
			Controls.Add(MkLabel("Пресет", LabelX, y));
			_nameBox = new TextBox { Location = new Point(FieldX, y - 2), Width = 280, Height = 26, Text = _preset.Name, Font = _uiFont };
			_nameBox.Leave += (_, __) => { _preset.Name = _nameBox.Text.Trim(); Save(); };
			Controls.Add(_nameBox);

			var btnDeletePreset = new Button { Text = "Удалить пресет", Location = new Point(ControlWidth - 130, y - 4), Size = new Size(120, 30), Font = _uiFont };
			btnDeletePreset.Click += (_, __) =>
			{
				_statsTimer.Stop();
				_worker.Dispose();
				_onDelete?.Invoke(_preset);
			};
			Controls.Add(btnDeletePreset);

			y += 36;
			Controls.Add(MkLabel("Цель", LabelX, y));
			_gameCombo = MkCombo(FieldX, y - 2);
			_gameCombo.SelectedIndexChanged += (_, __) =>
			{
				if (_suppressEvents) return;
				if (_gameCombo.SelectedItem is GameConfig g)
				{
					_preset.GameId = g.Id;
					Save();
					_worker.ReloadData();
				}
			};
			Controls.Add(_gameCombo);

			y += 36;
			Controls.Add(MkLabel("Промо", LabelX, y));
			_promoGameCombo = MkCombo(FieldX, y - 2);
			_promoGameCombo.SelectedIndexChanged += (_, __) =>
			{
				if (_suppressEvents) return;
				if (_promoGameCombo.SelectedItem is GameConfig g)
				{
					_preset.PromoGameId = g.Id;
					Save();
				}
			};
			Controls.Add(_promoGameCombo);

			y += 36;
			Controls.Add(MkLabel("Аккаунт", LabelX, y));
			_accountBox = new TextBox { Location = new Point(FieldX, y - 2), Width = 220, Height = 26, Text = _preset.SteamAccount, Font = _uiFont };
			_accountBox.Leave += (_, __) => { _preset.SteamAccount = _accountBox.Text.Trim(); Save(); _worker.ReloadData(); };
			Controls.Add(_accountBox);

			Controls.Add(MkLabel("Адрес", 330, y));
			_proxyHostBox = new TextBox { Location = new Point(385, y - 2), Width = 200, Height = 26, Text = _preset.ProxyHost, Font = _uiFont };
			_proxyHostBox.Leave += (_, __) => { _preset.ProxyHost = _proxyHostBox.Text.Trim(); Save(); };
			Controls.Add(_proxyHostBox);

			Controls.Add(MkLabel("Порт", 600, y));
			_proxyPortBox = new TextBox { Location = new Point(640, y - 2), Width = 70, Height = 26, Text = _preset.ProxyPort, Font = _uiFont };
			_proxyPortBox.Leave += (_, __) => { _preset.ProxyPort = _proxyPortBox.Text.Trim(); Save(); };
			Controls.Add(_proxyPortBox);

			_useProxyCheck = new CheckBox { Text = "Прокси", Location = new Point(720, y - 2), Checked = _preset.UseProxy, AutoSize = true, Font = _uiFont };
			_useProxyCheck.CheckedChanged += (_, __) => { _preset.UseProxy = _useProxyCheck.Checked; Save(); };
			Controls.Add(_useProxyCheck);

			y += 36;
			Controls.Add(MkLabel("Язык", LabelX, y));
			_langCombo = new ComboBox { Location = new Point(FieldX, y - 2), Width = 160, Height = 26, DropDownStyle = ComboBoxStyle.DropDownList, Font = _uiFont };
			_langCombo.Items.AddRange(SteamLanguages.All);
			_langCombo.SelectedItem = _preset.SelectedLanguage;
			if (_langCombo.SelectedIndex < 0) _langCombo.SelectedIndex = 0;
			_langCombo.SelectedIndexChanged += (_, __) =>
			{
				_preset.SelectedLanguage = _langCombo.SelectedItem?.ToString() ?? "all";
				Save();
			};
			Controls.Add(_langCombo);

			_continueCheck = new CheckBox
			{
				Text = "Продолжить сбор",
				Location = new Point(270, y - 2),
				Checked = _preset.ContinueCollection,
				AutoSize = true,
				Font = _uiFont
			};
			_continueCheck.CheckedChanged += (_, __) => { _preset.ContinueCollection = _continueCheck.Checked; Save(); };
			Controls.Add(_continueCheck);

			y += 36;
			_lblCollected = MkStatLabel("Собрано: 0", LabelX, y);
			_lblSpammed = MkStatLabel("Отправлено: 0", 200, y);
			_lblClosed = MkStatLabel("Закрыто: 0", 400, y);
			_lblDeleted = MkStatLabel("Удалено: 0", 580, y);
			Controls.AddRange(new Control[] { _lblCollected, _lblSpammed, _lblClosed, _lblDeleted });

			y += 28;
			Controls.Add(MkLabel("Статус", LabelX, y));
			_statusBox = new TextBox
			{
				Text = "Выключен",
				Location = new Point(FieldX, y - 4),
				Size = new Size(FieldWidth - 120, 56),
				ReadOnly = true,
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				BackColor = SystemColors.Window,
				ForeColor = Color.DarkBlue,
				Font = _uiFont,
				TabStop = true
			};
			_statusBox.MouseDown += (_, e) =>
			{
				if (e.Button == MouseButtons.Right && !string.IsNullOrEmpty(_statusBox.Text))
					_statusBox.SelectAll();
			};
			var statusMenu = new ContextMenuStrip();
			var copyItem = new ToolStripMenuItem("Копировать");
			copyItem.Click += (_, __) => CopyStatusToClipboard();
			statusMenu.Items.Add(copyItem);
			_statusBox.ContextMenuStrip = statusMenu;
			Controls.Add(_statusBox);

			_btnCopyStatus = new Button
			{
				Text = "Копировать",
				Location = new Point(ControlWidth - 125, y - 2),
				Size = new Size(110, 56),
				Font = _uiFont
			};
			_btnCopyStatus.Click += (_, __) => CopyStatusToClipboard();
			Controls.Add(_btnCopyStatus);

			y += 62;
			const int btnW = 118;
			const int gap = 8;
			int x = LabelX;
			_btnStart = MkButton("Запустить", x, y, btnW, (_, __) => _worker.StartChrome());
			x += btnW + gap;
			_btnStop = MkButton("Выключить", x, y, btnW, (_, __) => _worker.Stop());
			x += btnW + gap;
			_btnCollect = MkButton("Сбор", x, y, btnW, (_, __) => _worker.StartCollect());
			x += btnW + gap;
			_btnSpam = MkButton("Спам", x, y, btnW, (_, __) => _worker.StartSpam());
			x += btnW + gap;
			_btnDelete = MkButton("Удаление", x, y, btnW, (_, __) => _worker.StartDelete());
			Controls.AddRange(new Control[] { _btnStart, _btnStop, _btnCollect, _btnSpam, _btnDelete });
		}

		ComboBox MkCombo(int x, int y) =>
			new ComboBox { Location = new Point(x, y), Width = FieldWidth, Height = 26, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Name", Font = _uiFont };

		void OnStatusChanged(string status)
		{
			_statusBox.Text = status;
			_btnCopyStatus.Text = "Копировать";
			_statusBox.SelectionStart = 0;
			_statusBox.SelectionLength = 0;
			UpdateButtonStates();
		}

		void CopyStatusToClipboard()
		{
			if (string.IsNullOrWhiteSpace(_statusBox.Text))
				return;
			try
			{
				Clipboard.SetText(_statusBox.Text);
				_btnCopyStatus.Text = "Скопировано!";
			}
			catch (Exception ex)
			{
				_statusBox.Text += "\r\n\r\nНе удалось скопировать: " + ex.Message;
			}
		}

		void UpdateButtonStates()
		{
			var chrome = _worker.IsChromeRunning;
			var busy = _worker.IsBusy;

			_btnStart.Enabled = !busy;
			_btnStop.Enabled = chrome || busy;
			_btnCollect.Enabled = chrome && !busy;
			_btnSpam.Enabled = chrome && !busy;
			_btnDelete.Enabled = chrome && !busy;
		}

		public void RefreshGameList()
		{
			_suppressEvents = true;
			FillGameCombo(_gameCombo, _preset.GameId);
			FillGameCombo(_promoGameCombo, _preset.PromoGameId);
			_suppressEvents = false;
		}

		void FillGameCombo(ComboBox combo, string selectedId)
		{
			combo.Items.Clear();
			foreach (var g in _getGames())
				combo.Items.Add(g);

			if (!string.IsNullOrEmpty(selectedId))
			{
				for (int i = 0; i < combo.Items.Count; i++)
				{
					if (combo.Items[i] is GameConfig g && g.Id == selectedId)
					{
						combo.SelectedIndex = i;
						return;
					}
				}
			}

			if (combo.Items.Count > 0)
				combo.SelectedIndex = 0;
		}

		void UpdateStats(GameStats stats)
		{
			_lblCollected.Text = "Собрано: " + stats.Collected;
			_lblSpammed.Text = "Отправлено: " + stats.Spammed;
			_lblClosed.Text = "Закрыто: " + stats.Closed;
			_lblDeleted.Text = "Удалено: " + stats.Deleted;
		}

		void Save()
		{
			PresetStore.Save(_preset);
			_onChanged?.Invoke(_preset);
		}

		public void DisposeWorker()
		{
			_statsTimer?.Stop();
			_statsTimer?.Dispose();
			_worker?.Dispose();
		}

		Label MkLabel(string text, int x, int y) =>
			new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = _uiFont };

		Label MkStatLabel(string text, int x, int y) =>
			new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

		Button MkButton(string text, int x, int y, int width, EventHandler click)
		{
			var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 32), Font = _uiFont };
			b.Click += click;
			return b;
		}
	}
}
