﻿
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
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.button8 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.button9 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.button11 = new System.Windows.Forms.Button();
			this.button12 = new System.Windows.Forms.Button();
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
			this.button3.Location = new System.Drawing.Point(188, 151);
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
			this.textBox1.Text = "https://steamcommunity.com/app/361420/reviews/";
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
			this.textBox2.Text = "ASTRONEER";
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(39, 376);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(132, 43);
			this.button6.TabIndex = 14;
			this.button6.Text = "Завершить";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click_1);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(39, 278);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(132, 43);
			this.button7.TabIndex = 13;
			this.button7.Text = "Старт";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(188, 278);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(132, 23);
			this.textBox3.TabIndex = 16;
			this.textBox3.Text = "Astroneer";
			this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged_1);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(188, 260);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(59, 15);
			this.label4.TabIndex = 15;
			this.label4.Text = "Название";
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(39, 327);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(132, 43);
			this.button8.TabIndex = 17;
			this.button8.Text = "Начать спам";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.button8_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(188, 327);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 15);
			this.label6.TabIndex = 31;
			this.label6.Text = "label6";
			this.label6.Click += new System.EventHandler(this.label6_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(755, 205);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 15);
			this.label5.TabIndex = 32;
			this.label5.Text = "label5";
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(617, 205);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(132, 23);
			this.textBox4.TabIndex = 33;
			this.textBox4.Text = "https://www.youtube.com/watch?v=s1PGBhGK9WY";
			this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(617, 187);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(40, 15);
			this.label7.TabIndex = 34;
			this.label7.Text = "Видео";
			this.label7.Click += new System.EventHandler(this.label7_Click);
			// 
			// button9
			// 
			this.button9.Location = new System.Drawing.Point(479, 205);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(132, 43);
			this.button9.TabIndex = 35;
			this.button9.Text = "Старт";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(this.button9_Click);
			// 
			// button10
			// 
			this.button10.Location = new System.Drawing.Point(479, 303);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(132, 43);
			this.button10.TabIndex = 36;
			this.button10.Text = "Сделать токены";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(this.button10_Click);
			// 
			// button11
			// 
			this.button11.Location = new System.Drawing.Point(479, 352);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(132, 43);
			this.button11.TabIndex = 37;
			this.button11.Text = "Логинизация";
			this.button11.UseVisualStyleBackColor = true;
			this.button11.Click += new System.EventHandler(this.button11_Click);
			// 
			// button12
			// 
			this.button12.Location = new System.Drawing.Point(479, 254);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(132, 43);
			this.button12.TabIndex = 38;
			this.button12.Text = "Спам";
			this.button12.UseVisualStyleBackColor = true;
			this.button12.Click += new System.EventHandler(this.button12_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(970, 450);
			this.Controls.Add(this.button12);
			this.Controls.Add(this.button11);
			this.Controls.Add(this.button10);
			this.Controls.Add(this.button9);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button7);
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
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.Button button12;
	}
}
