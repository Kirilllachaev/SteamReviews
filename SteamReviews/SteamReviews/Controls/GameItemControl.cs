using System;
using System.Drawing;
using System.Windows.Forms;
using SteamReviews.Models;
using SteamReviews.Services;

namespace SteamReviews.Controls
{
	public class GameItemControl : UserControl
	{
		readonly GameConfig _game;
		readonly Action<GameConfig> _onChanged;
		readonly Action<GameConfig> _onDelete;

		TextBox _nameBox;
		TextBox _linkBox;

		public GameItemControl(GameConfig game, Action<GameConfig> onChanged, Action<GameConfig> onDelete)
		{
			_game = game;
			_onChanged = onChanged;
			_onDelete = onDelete;
			BuildUi();
		}

		void BuildUi()
		{
			Size = new Size(440, 130);
			BorderStyle = BorderStyle.FixedSingle;
			Margin = new Padding(6);
			Font = new Font("Segoe UI", 10);

			var lblName = new Label { Text = "Название", Location = new Point(10, 10), AutoSize = true };
			_nameBox = new TextBox
			{
				Location = new Point(10, 30),
				Width = 415,
				Height = 26,
				Text = _game.Name,
				Font = Font
			};
			_nameBox.Leave += (_, __) => ApplyNameChange();

			var lblLink = new Label { Text = "Ссылка Steam", Location = new Point(10, 62), AutoSize = true };
			_linkBox = new TextBox
			{
				Location = new Point(10, 82),
				Width = 330,
				Height = 26,
				Text = _game.StoreLink,
				Font = Font
			};
			_linkBox.Leave += (_, __) =>
			{
				_game.StoreLink = _linkBox.Text.Trim();
				_onChanged?.Invoke(_game);
			};

			var btnDelete = new Button
			{
				Text = "Удалить",
				Location = new Point(350, 80),
				Size = new Size(75, 30),
				Font = Font
			};
			btnDelete.Click += (_, __) => _onDelete?.Invoke(_game);

			Controls.AddRange(new Control[] { lblName, _nameBox, lblLink, _linkBox, btnDelete });
		}

		void ApplyNameChange()
		{
			var newName = _nameBox.Text.Trim();
			if (string.IsNullOrEmpty(newName) || newName == _game.Name)
				return;

			var oldFolder = AppPaths.GameFolder(_game.Name);
			var newFolder = AppPaths.GameFolder(newName);

			if (System.IO.Directory.Exists(oldFolder) && !System.IO.Directory.Exists(newFolder))
				System.IO.Directory.Move(oldFolder, newFolder);
			else if (!System.IO.Directory.Exists(newFolder))
				System.IO.Directory.CreateDirectory(newFolder);

			_game.Name = newName;
			_onChanged?.Invoke(_game);
		}
	}
}
