
namespace SteamReviews
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button5 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.button6 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.button7 = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Worked = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(188, 53);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(132, 43);
			this.button2.TabIndex = 1;
			this.button2.Text = "Начать сбор";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(188, 249);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(132, 43);
			this.button3.TabIndex = 2;
			this.button3.Text = "Остановить";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(39, 53);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(132, 43);
			this.button1.TabIndex = 3;
			this.button1.Text = "Старт";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(39, 102);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(132, 43);
			this.button4.TabIndex = 4;
			this.button4.Text = "Завершить";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "all",
            "italian",
            "polish",
            "schinese",
            "tchinese",
            "japanese",
            "koreana",
            "thai",
            "bulgarian",
            "czech",
            "danish",
            "german",
            "english",
            "spanish",
            "latam",
            "greek",
            "french",
            "hungarian",
            "dutch",
            "norwegian",
            "portuguese",
            "brazilian",
            "romanian",
            "russian",
            "finnish",
            "swedish",
            "turkish",
            "vietnamese",
            "ukrainian"});
			this.comboBox1.Location = new System.Drawing.Point(617, 55);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(132, 23);
			this.comboBox1.TabIndex = 5;
			this.comboBox1.Text = "all";
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(617, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 15);
			this.label1.TabIndex = 6;
			this.label1.Text = "Язык";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(188, 102);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(132, 43);
			this.button5.TabIndex = 7;
			this.button5.Text = "Начать спам";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(479, 55);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(132, 23);
			this.textBox1.TabIndex = 9;
			this.textBox1.Text = "https://steamcommunity.com/app/1743830/reviews/";
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(479, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 15);
			this.label2.TabIndex = 10;
			this.label2.Text = "Ссылка";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(341, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 15);
			this.label3.TabIndex = 11;
			this.label3.Text = "Название";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(341, 55);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(132, 23);
			this.textBox2.TabIndex = 12;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(341, 81);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 15);
			this.label4.TabIndex = 37;
			this.label4.Text = "Count:";
			this.label4.Click += new System.EventHandler(this.label4_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(341, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 15);
			this.label5.TabIndex = 38;
			this.label5.Text = "Spammed:";
			this.label5.Click += new System.EventHandler(this.label5_Click_1);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(341, 111);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 15);
			this.label6.TabIndex = 39;
			this.label6.Text = "Closed";
			this.label6.Click += new System.EventHandler(this.label6_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(188, 200);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(132, 43);
			this.button6.TabIndex = 40;
			this.button6.Text = "Удалить коммы";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click_1);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(479, 84);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(132, 23);
			this.textBox3.TabIndex = 41;
			this.textBox3.Text = "EL FUGADO2";
			this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged_1);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(479, 110);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(47, 15);
			this.label7.TabIndex = 42;
			this.label7.Text = "Deleted";
			this.label7.Click += new System.EventHandler(this.label7_Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(188, 151);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(132, 43);
			this.button7.TabIndex = 43;
			this.button7.Text = "Собрать коммы";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Worked,
            this.Column3,
            this.Column4});
			this.dataGridView1.Location = new System.Drawing.Point(341, 151);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 25;
			this.dataGridView1.Size = new System.Drawing.Size(658, 276);
			this.dataGridView1.TabIndex = 44;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// Column1
			// 
			this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Column1.HeaderText = "Game";
			this.Column1.MinimumWidth = 200;
			this.Column1.Name = "Column1";
			this.Column1.Width = 200;
			// 
			// Column2
			// 
			this.Column2.HeaderText = "Positive";
			this.Column2.Name = "Column2";
			// 
			// Worked
			// 
			this.Worked.HeaderText = "Worked";
			this.Worked.Name = "Worked";
			// 
			// Column3
			// 
			this.Column3.HeaderText = "Spammed";
			this.Column3.Name = "Column3";
			// 
			// Column4
			// 
			this.Column4.HeaderText = "Closed";
			this.Column4.Name = "Column4";
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(755, 53);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(132, 23);
			this.textBox4.TabIndex = 45;
			this.textBox4.Text = "0";
			this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(224, 35);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(96, 19);
			this.checkBox1.TabIndex = 46;
			this.checkBox1.Text = "Продолжить";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(39, 24);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(132, 23);
			this.textBox5.TabIndex = 47;
			this.textBox5.Text = "82.211.9.190:59100";
			this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new System.Drawing.Point(224, 10);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(68, 19);
			this.checkBox2.TabIndex = 48;
			this.checkBox2.Text = "Прокси";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1189, 450);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.textBox5);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Steam Reviews";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Worked;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
	}
}

