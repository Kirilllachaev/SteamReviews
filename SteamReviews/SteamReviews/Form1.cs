using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SteamReviews.Controls;
using SteamReviews.Models;
using SteamReviews.Services;

namespace SteamReviews
{
	public partial class Form1 : Form
	{
		List<GameConfig> _games = new List<GameConfig>();
		List<ProcessPreset> _presets = new List<ProcessPreset>();

		FlowLayoutPanel _gamesPanel;
		FlowLayoutPanel _presetsPanel;

		public Form1()
		{
			InitializeComponent();
			LoadData();
			BuildLists();
		}

		void LoadData()
		{
			AppPaths.EnsureDirectories();
			_games = GameStore.Load();
			GameStore.Save(_games);
			_presets = PresetStore.LoadAll();
		}

		void BuildLists()
		{
			DisposeAllWorkers();
			_gamesPanel.Controls.Clear();
			foreach (var game in _games)
				_gamesPanel.Controls.Add(new GameItemControl(game, OnGameChanged, OnGameDelete));

			_presetsPanel.Controls.Clear();
			foreach (var preset in _presets)
				AddPresetControl(preset);
		}

		void DisposeAllWorkers()
		{
			foreach (Control c in _presetsPanel.Controls)
			{
				if (c is ProcessPresetControl pc)
					pc.DisposeWorker();
			}
		}

		void AddPresetControl(ProcessPreset preset)
		{
			var ctrl = new ProcessPresetControl(preset, () => _games, OnPresetChanged, OnPresetDelete);
			_presetsPanel.Controls.Add(ctrl);
		}

		void OnGameChanged(GameConfig game)
		{
			GameStore.Save(_games);
			RefreshAllPresetGameLists();
		}

		void OnGameDelete(GameConfig game)
		{
			if (MessageBox.Show("Удалить игру «" + game.Name + "» и её папку?", "Подтверждение",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			GameStore.Delete(game, _games);
			GameStore.Save(_games);

			foreach (var preset in _presets.Where(p => p.GameId == game.Id))
			{
				preset.GameId = "";
				PresetStore.Save(preset);
			}

			BuildLists();
		}

		void OnPresetChanged(ProcessPreset preset) { }

		void OnPresetDelete(ProcessPreset preset)
		{
			if (MessageBox.Show("Удалить пресет «" + preset.Name + "»?", "Подтверждение",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			foreach (Control c in _presetsPanel.Controls)
			{
				if (c is ProcessPresetControl pc && (string)pc.Tag == preset.Id)
				{
					pc.DisposeWorker();
					_presetsPanel.Controls.Remove(pc);
					break;
				}
			}

			PresetStore.Delete(preset);
			_presets.RemoveAll(p => p.Id == preset.Id);
		}

		void RefreshAllPresetGameLists()
		{
			foreach (Control c in _presetsPanel.Controls)
			{
				if (c is ProcessPresetControl pc)
					pc.RefreshGameList();
			}
		}

		void BtnAddGame_Click(object sender, EventArgs e)
		{
			using var dlg = new Form
			{
				Text = "Новая игра",
				Size = new Size(620, 240),
				StartPosition = FormStartPosition.CenterParent,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MaximizeBox = false,
				MinimizeBox = false,
				Font = new Font("Segoe UI", 10)
			};

			var lblName = new Label { Text = "Название", Location = new Point(16, 20), AutoSize = true };
			var tbName = new TextBox { Location = new Point(16, 42), Width = 570, Height = 28 };
			var lblLink = new Label { Text = "Ссылка Steam", Location = new Point(16, 82), AutoSize = true };
			var tbLink = new TextBox { Location = new Point(16, 104), Width = 570, Height = 28 };
			var btnOk = new Button { Text = "Создать", Location = new Point(380, 155), Size = new Size(100, 34), DialogResult = DialogResult.OK };
			var btnCancel = new Button { Text = "Отмена", Location = new Point(490, 155), Size = new Size(100, 34), DialogResult = DialogResult.Cancel };
			dlg.Controls.AddRange(new Control[] { lblName, tbName, lblLink, tbLink, btnOk, btnCancel });
			dlg.AcceptButton = btnOk;
			dlg.CancelButton = btnCancel;

			if (dlg.ShowDialog(this) != DialogResult.OK)
				return;

			var name = tbName.Text.Trim();
			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show("Введите название игры");
				return;
			}

			if (_games.Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
			{
				MessageBox.Show("Игра с таким названием уже есть");
				return;
			}

			var game = GameStore.Add(name, tbLink.Text.Trim());
			_games.Add(game);
			GameStore.Save(_games);
			_gamesPanel.Controls.Add(new GameItemControl(game, OnGameChanged, OnGameDelete));
			RefreshAllPresetGameLists();
		}

		void BtnAddPreset_Click(object sender, EventArgs e)
		{
			var preset = new ProcessPreset();
			if (_games.Count > 0)
			{
				preset.GameId = _games[0].Id;
				preset.PromoGameId = _games.Count > 1 ? _games[1].Id : _games[0].Id;
			}

			PresetStore.Save(preset);
			_presets.Add(preset);
			AddPresetControl(preset);
		}

		void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			DisposeAllWorkers();
		}
	}
}
