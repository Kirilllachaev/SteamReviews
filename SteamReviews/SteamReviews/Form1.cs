using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;

namespace SteamReviews
{

	

	public partial class Form1 : Form
	{

		public class DiscordDriver
		{
			public string Log = "";
			public string pas = "";
			public Thread DiscrodSpamThread = null;
			public ChromeDriver DiscrodDriver = null;
			public int Seconds = 0;


		
		}

		public Form1()
		{
			InitializeComponent();

			selectedLanguage = comboBox1.Text;


		}

		ChromeDriver driver = null;
		bool isScrolling = false;
		bool isCollecting = false;
		Thread ScrollingThread = null;
		Thread MultipleLangsThread = null;

		public int pageNumber = 0;

		public List<string> Alllinks = new List<string>();

		public string selectedLanguage = "all";


		public string gameName = "";
		public string gameLink = "";



		public string linksFile = "";

		public int allLinkCount = 0;


		#region Steam

		private void button1_Click_1(object sender, EventArgs e)
		{

			if (driver != null)
				driver.Quit();

			driver = new ChromeDriver();
			driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");




		}








		private void button2_Click(object sender, EventArgs e)
		{

			gameName = textBox2.Text;
			gameLink = textBox1.Text;


			if (selectedLanguage == "all")
			{
				MultipleLangsThread = new Thread(() => MultipleLangs());
				MultipleLangsThread.Start();
				isCollecting = true;
			}
			else
			{
				//	driver = new ChromeDriver();

				driver.Navigate().GoToUrl(gameLink + "/?p=1&browsefilter=all&filterLanguage=" + selectedLanguage);

				Thread.Sleep(1000);

				try
				{
					Thread.Sleep(500);
					IWebElement checkich = driver.FindElement(By.Id("ViewAllForApp"));
					checkich.Click();
					Thread.Sleep(500);
					IWebElement grayButton = driver.FindElement(By.Id("age_gate_btn_continue"));
					grayButton.Click();
					Thread.Sleep(500);

				}
				catch
				{

				}

				isScrolling = true;

				ScrollingThread = new Thread(() => Scrolling(driver, true, selectedLanguage));
				ScrollingThread.Start();
				isCollecting = true;


			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			isScrolling = false;

			if (selectedLanguage != "all")
				MakeOutput(gameName, selectedLanguage);

			pageNumber = 0;
			allLinkCount = 0;
			Alllinks.Clear();

			isCollecting = false;


			driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");



		}



		private void button4_Click(object sender, EventArgs e)
		{
			if (driver != null)
				driver.Quit();

			Application.Exit();
		}



		private void button5_Click(object sender, EventArgs e)
		{
			IWebElement textarea = driver.FindElement(By.TagName("textarea"));
			textarea.SendKeys("This is a sample text.");

			IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
			greenButton.Click();
		}

		private void button6_Click(object sender, EventArgs e)
		{

		}

		public void MultipleLangs()
		{

			for (int i = 1; i < comboBox1.Items.Count; i++)
			{

				if (!isCollecting)
					return;

				string CselectedLanguage = comboBox1.Items[i].ToString();

				//driver = new ChromeDriver();
				driver.Navigate().GoToUrl(gameLink + "/?p=1&browsefilter=all&filterLanguage=" + CselectedLanguage);

				Thread.Sleep(1000);


				try
				{
					Thread.Sleep(500);
					IWebElement checkich = driver.FindElement(By.Id("ViewAllForApp"));
					checkich.Click();
					Thread.Sleep(500);
					IWebElement grayButton = driver.FindElement(By.Id("age_gate_btn_continue"));
					grayButton.Click();
					Thread.Sleep(500);

				}
				catch
				{

				}



				isScrolling = true;
				Scrolling(driver, false, CselectedLanguage);

				MakeOutput(gameName, CselectedLanguage);



				//driver.Quit();


			}

			Thread t = new Thread(() => MessageBox.Show("Сбор ссылок завершён. Было собрано " + allLinkCount.ToString() + " ссылок", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information));
			t.Start();

			Thread.Sleep(500);


		}




		public void Scrolling(ChromeDriver myDriver, bool One, string _selectedLanguage)
		{
			while (isScrolling)
			{

				try
				{
					((IJavaScriptExecutor)myDriver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

					Thread.Sleep(100);

					pageNumber += 1;
					IWebElement page = myDriver.FindElement(By.Id("page" + pageNumber.ToString()));

					ReadOnlyCollection<IWebElement> childElements = page.FindElements(By.ClassName("apphub_Card"));
					IWebElement[] cards = childElements.ToArray();

					foreach (IWebElement E in cards)
					{
						string link = E.GetAttribute("data-modal-content-url");
						Alllinks.Add(link);
					}

					Thread.Sleep(1000);

				}
				catch
				{

					try
					{
						IWebElement end = myDriver.FindElement(By.ClassName("apphub_NoMoreContentText1"));

						isScrolling = false;


						if (One)
						{
							MakeOutput(gameName, _selectedLanguage);
						}

					}
					catch
					{

					}



				}

			}
		}


		public void MakeOutput(string _gameName, string _selectedLanguage)
		{

			if (System.IO.Directory.Exists(@_gameName))
			{
				System.IO.File.WriteAllLines(@"Steam/" + _gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
			}
			else
			{
				System.IO.Directory.CreateDirectory(gameName);
				System.IO.File.WriteAllLines(@"Steam/" + _gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
			}

			allLinkCount += Alllinks.Count;

			Thread t = new Thread(() => MessageBox.Show("Сбор языка " + _selectedLanguage + " завершён. Было собрано " + Alllinks.Count + " ссылок", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning));
			t.Start();

			Thread.Sleep(500);

			pageNumber = 0;
			Alllinks.Clear();

		}





		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedLanguage = comboBox1.Text;
		}



		#endregion





		#region Discord

		public List<string> DiscordMembers;
		public string _ServerName = "";

		string MessageText = "";
		string MessageText2 = "";

		public List<DiscordDriver> DiscordDrivers;


		public void MakeDD()
		{
			int _num = DiscordDrivers.Count;

			DiscordDriver DD = new DiscordDriver();
			if(_num == 0)
			{
				DD.Log = textBox4.Text;
				DD.pas = textBox5.Text;			
			}
			if (_num == 1)
			{
				DD.Log = textBox6.Text;
				DD.pas = textBox7.Text;
			}
			if (_num == 2)
			{
				DD.Log = textBox8.Text;
				DD.pas = textBox9.Text;
			}
			if (_num == 3)
			{
				DD.Log = textBox10.Text;
				DD.pas = textBox11.Text;
			}
			if (_num == 4)
			{
				DD.Log = textBox12.Text;
				DD.pas = textBox13.Text;
			}
			if (_num == 5)
			{
				DD.Log = textBox14.Text;
				DD.pas = textBox15.Text;
			}

		
			DiscordDrivers.Add(DD);
			DD.DiscrodDriver = new ChromeDriver();
			DD.DiscrodDriver.Navigate().GoToUrl("https://discord.com/login");

			Thread.Sleep(10000);


			IWebElement LoginArea = DD.DiscrodDriver.FindElement(By.CssSelector("[aria-label='Адрес электронной почты или номер телефона']"));
			IWebElement PassArea = DD.DiscrodDriver.FindElement(By.CssSelector("[aria-label='Пароль']"));
			LoginArea.SendKeys(DD.Log);
			PassArea.SendKeys(DD.pas);

			Thread.Sleep(2000);

			IWebElement ButtonArea = DD.DiscrodDriver.FindElement(By.ClassName("button-1cRKG6"));

			ButtonArea.Click();

			Thread.Sleep(10000);

			DD.DiscrodSpamThread = new Thread(() => DiscordSpam(DD, DiscordMembers));
			DD.DiscrodSpamThread.Start();


		}


		private void button7_Click(object sender, EventArgs e)
		{

			MessageText = File.ReadAllText(@"Discord/Message.txt");
			MessageText2 = File.ReadAllText(@"Discord/Message2.txt");
		

			_ServerName = textBox3.Text;
			DiscordMembers = new List<string>();
			using (StreamReader reader = new StreamReader(@"Discord/" + _ServerName + ".txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					DiscordMembers.Add(line);
				}
			}

			MessageBox.Show("Загружено " + DiscordMembers.Count + " ссылок");

			DiscordDrivers = new List<DiscordDriver>();


			for(int i = 0; i < 6; i++)
			{
				MakeDD();

				Thread.Sleep(5000);
			}
		
/*
			// Открыть страницу что бы залогиниться
			if (driver != null)
				driver.Quit();

			driver = new ChromeDriver();
			driver.Navigate().GoToUrl("https://discord.com/login");*/





			/*Thread.Sleep(4000);


			IWebElement LoginArea = driver.FindElement(By.CssSelector("[aria-label='Адрес электронной почты или номер телефона']"));
			IWebElement PassArea = driver.FindElement(By.CssSelector("[aria-label='Пароль']"));
			LoginArea.SendKeys("santosrosarbz@rambler.ru");
			PassArea.SendKeys("54nsG6g8jh");

			Thread.Sleep(1000);

			IWebElement ButtonArea = driver.FindElement(By.ClassName("button-1cRKG6"));

			ButtonArea.Click();*/





		}

		private void button8_Click(object sender, EventArgs e)
		{
			
			// Начать спам
			

		//	DiscrodSpamThread = new Thread(() => DiscordSpam(DiscrodDriver1, DiscordMembers));
			//DiscrodSpamThread.Start();

		


		}

		private void button6_Click_1(object sender, EventArgs e)
		{
			//Завершить
			if (driver != null)
				driver.Quit();

			Application.Exit();
		}

		private void textBox3_TextChanged_1(object sender, EventArgs e)
		{
			// Название файлика
		}


		public void DiscordSpam(DiscordDriver drive, List<string> Members)
		{

		

			for (int i = 1; i < Members.Count; i++)
			{
				if (Members[i].Contains("☻") && (i % DiscordDrivers.IndexOf(drive)+1 != 0))
				{
					return;
				}

				drive.DiscrodDriver.Navigate().GoToUrl("https://discord.com/users/" + DiscordMembers[i]);
				Thread.Sleep(8000);

				IWebElement ButtonParent = drive.DiscrodDriver.FindElement(By.ClassName("relationshipButtons-3ByBpj"));
				IWebElement ButtonArea = ButtonParent.FindElement(By.CssSelector("[role = 'button']"));

				ButtonArea.Click();
				Thread.Sleep(1000);

				IWebElement ButtonMessage = drive.DiscrodDriver.FindElement(By.Id("user-profile-actions-user-message"));
				ButtonMessage.Click();
				Thread.Sleep(8000);

				IWebElement TextParent = drive.DiscrodDriver.FindElement(By.ClassName("emptyText-1o0WH_"));
				IWebElement TextArea = TextParent.FindElement(By.CssSelector("span"));

				if(i % 2 == 0)
				{
					TextArea.SendKeys(MessageText);
				}
				else
				{
					TextArea.SendKeys(MessageText2);
				}
				




				Thread.Sleep(1000);

				Actions actions = new Actions(drive.DiscrodDriver);
				actions.SendKeys(OpenQA.Selenium.Keys.Enter).Perform();

				DiscordMembers[i] += "☻";

				File.WriteAllText(@"Discord/" + _ServerName + ".txt", "");

				using (StreamWriter writer = new StreamWriter(@"Discord/" + _ServerName + ".txt"))
				{
					foreach (string line in DiscordMembers)
					{
						writer.WriteLine(line);
					}
				}

				/*if(i % 9 == 0)
				{
					Thread t = new Thread(() => MessageBox.Show("Написано 10 сообщений. Ожидание 610 сек"));
					t.Start();
					Thread.Sleep(660000);
				}
				else
				{
							
				}*/

				drive.Seconds = 660;
				Thread.Sleep(660000);

				


			}
		}




		#endregion


		#region NotUse

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			driver.Quit();
		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		#endregion

		private void timer1_Tick(object sender, EventArgs e)
		{
			if(DiscordDrivers != null)
			for(int i = 0; i < DiscordDrivers.Count; i++)
			{
				if(DiscordDrivers[i].Seconds > 0)
				{
					DiscordDrivers[i].Seconds -= 1;

					if (i == 0)
					{
						label6.Text = DiscordDrivers[i].Seconds.ToString();
					}
					if (i == 1)
					{
						label7.Text = DiscordDrivers[i].Seconds.ToString();

					}
					if (i == 2)
					{
						label8.Text = DiscordDrivers[i].Seconds.ToString();

					}
					if (i == 3)
					{
						label9.Text = DiscordDrivers[i].Seconds.ToString();

					}
					if (i == 4)
					{
						label10.Text = DiscordDrivers[i].Seconds.ToString();

					}
					if (i == 5)
					{
						label11.Text = DiscordDrivers[i].Seconds.ToString();

					}

				}
			}
		}

		private void label5_Click_1(object sender, EventArgs e)
		{

		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox5_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox6_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox7_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox8_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox9_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox10_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox11_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox13_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox12_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox15_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox14_TextChanged(object sender, EventArgs e)
		{

		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		private void label8_Click(object sender, EventArgs e)
		{

		}

		private void label9_Click(object sender, EventArgs e)
		{

		}

		private void label10_Click(object sender, EventArgs e)
		{

		}

		private void label11_Click(object sender, EventArgs e)
		{

		}
	}
}
