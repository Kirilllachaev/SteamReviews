namespace SteamReviews
{
	partial class Form1
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();

			var split = new System.Windows.Forms.SplitContainer
			{
				Dock = System.Windows.Forms.DockStyle.Fill,
				Orientation = System.Windows.Forms.Orientation.Vertical,
				SplitterDistance = 480,
				FixedPanel = System.Windows.Forms.FixedPanel.Panel1
			};

			// Games panel (left)
			var gamesHeader = new System.Windows.Forms.Panel
			{
				Dock = System.Windows.Forms.DockStyle.Top,
				Height = 48
			};
			var lblGames = new System.Windows.Forms.Label
			{
				Text = "Игры",
				Location = new System.Drawing.Point(10, 14),
				AutoSize = true,
				Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold)
			};
			var btnAddGame = new System.Windows.Forms.Button
			{
				Text = "+ Добавить игру",
				Location = new System.Drawing.Point(100, 8),
				Size = new System.Drawing.Size(140, 32),
				Font = new System.Drawing.Font("Segoe UI", 10)
			};
			btnAddGame.Click += new System.EventHandler(this.BtnAddGame_Click);
			gamesHeader.Controls.Add(lblGames);
			gamesHeader.Controls.Add(btnAddGame);

			var gamesScroll = new System.Windows.Forms.Panel
			{
				Dock = System.Windows.Forms.DockStyle.Fill,
				AutoScroll = true
			};
			_gamesPanel = new System.Windows.Forms.FlowLayoutPanel
			{
				Dock = System.Windows.Forms.DockStyle.Top,
				AutoSize = true,
				FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
				WrapContents = false,
				Width = 460
			};
			gamesScroll.Controls.Add(_gamesPanel);
			split.Panel1.Controls.Add(gamesScroll);
			split.Panel1.Controls.Add(gamesHeader);

			// Presets panel (right)
			var presetsHeader = new System.Windows.Forms.Panel
			{
				Dock = System.Windows.Forms.DockStyle.Top,
				Height = 48
			};
			var lblPresets = new System.Windows.Forms.Label
			{
				Text = "Пресеты процессов",
				Location = new System.Drawing.Point(10, 14),
				AutoSize = true,
				Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold)
			};
			var btnAddPreset = new System.Windows.Forms.Button
			{
				Text = "+ Добавить пресет",
				Location = new System.Drawing.Point(200, 8),
				Size = new System.Drawing.Size(150, 32),
				Font = new System.Drawing.Font("Segoe UI", 10)
			};
			btnAddPreset.Click += new System.EventHandler(this.BtnAddPreset_Click);
			presetsHeader.Controls.Add(lblPresets);
			presetsHeader.Controls.Add(btnAddPreset);

			var presetsScroll = new System.Windows.Forms.Panel
			{
				Dock = System.Windows.Forms.DockStyle.Fill,
				AutoScroll = true
			};
			_presetsPanel = new System.Windows.Forms.FlowLayoutPanel
			{
				Dock = System.Windows.Forms.DockStyle.Top,
				AutoSize = true,
				FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
				WrapContents = false,
				Width = 880
			};
			presetsScroll.Controls.Add(_presetsPanel);
			split.Panel2.Controls.Add(presetsScroll);
			split.Panel2.Controls.Add(presetsHeader);

			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1420, 860);
			this.MinimumSize = new System.Drawing.Size(1200, 700);
			this.Font = new System.Drawing.Font("Segoe UI", 10);
			this.Controls.Add(split);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Steam Reviews";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
		}
	}
}
