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
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;
using OpenQA.Selenium.Remote;

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


		#region YouTube


		public class YTClass
		{
			public string Log = "";
			public string pas = "";
			public int Replies = 0;
			public string myname = "";
			public string nomination = "";
			public string working = "";
			public Thread YTThread = null;
			public IWebDriver YTDriver = null;
		
		}


		public List<YTClass> YTDrivers;
		public List<string> YouTubeTokens;
		public string VideoLink = "";
		string YTMessageText = "";
		public int Replies;

		//public FirefoxDriverService YToptions;
		public ChromeOptions YToptions = new ChromeOptions();


		private void button9_Click(object sender, EventArgs e)
		{
			// Старт
			YTDrivers = new List<YTClass>();

			VideoLink = textBox4.Text;
			YTMessageText = File.ReadAllText(@"YouTube/Message.txt");




			YouTubeTokens = new List<string>();
			using (StreamReader reader = new StreamReader(@"YouTube/Tokens.txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					YouTubeTokens.Add(line);
				}
			}


			foreach (string T in YouTubeTokens)
			{
				YTClass DD = new YTClass();

				DD.Log = T.Split(":")[0];
				DD.pas = T.Split(":")[1];
				DD.myname = T.Split(":")[2];
				DD.nomination = T.Split(":")[3];
				DD.working = T.Split(":")[4];
				DD.Replies = Int32.Parse(T.Split(":")[5]);

				YTDrivers.Add(DD);
			}


			MessageBox.Show("Готово к работе");




		}


		public void GetTokens()
		{

			

			YouTubeTokens = new List<string>();
			using (StreamReader reader = new StreamReader(@"YouTube/Tokens.txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					YouTubeTokens.Add(line);
				}
			}

			YTDrivers = new List<YTClass>();

			foreach (string T in YouTubeTokens)
			{
				YTClass DD = new YTClass();

				DD.Log = T.Split(":")[0];
				DD.pas = T.Split(":")[1];
				DD.myname = T.Split(":")[2];
				DD.nomination = T.Split(":")[3];
				DD.working = T.Split(":")[4];
				DD.Replies = Int32.Parse(T.Split(":")[5]);

				YTDrivers.Add(DD);
			}
		}

		public void SetTokens()
		{
			File.WriteAllText(@"YouTube/Tokens.txt", "");

			using (StreamWriter writer = new StreamWriter(@"YouTube/Tokens.txt"))
			{
				foreach (YTClass Y in YTDrivers)
				{

					writer.WriteLine(Y.Log + ":" + Y.pas + ":" + Y.myname + ":" + Y.nomination + ":" + Y.working + ":" + Y.Replies.ToString());
				}
			}
		}

		private void button11_Click(object sender, EventArgs e)
		{

			bool ready = true;
			//Логинизация

			while (ready)
			{
				ready = CheckAccounts();
			}
			

			//Если есть ahT6S 
		}


		public bool CheckAccounts()
		{
			YToptions.AddArgument("user-data-dir=C:\\Users\\73961\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 2");
		


			GetTokens();

			YTClass DD = null;

			foreach (YTClass YTC in YTDrivers)
			{
				if (YTC.working == "Yes")
				{
					DD = YTC;
					DD.working = "Checking";
					SetTokens();
					break;
				}
			}

			if (DD == null)
			{
				MessageBox.Show("Все токены проверены");
				return false;
			}


			if (DD.YTDriver != null)
				DD.YTDriver.Quit();

			DD.YTDriver = new ChromeDriver(YToptions);

			//ChromeOptions options = new ChromeOptions();
			//options.DebuggerAddress = "127.0.0.1:1234"; // адрес отладчика Chrome
			//DD.YTDriver = new RemoteWebDriver(new Uri("http://localhost:1234/wd/hub"), options.ToCapabilities());

			DD.YTDriver.Navigate().GoToUrl("https://accounts.google.com/v3/signin/identifier?dsh=S2047072606%3A1683301289488608&continue=https%3A%2F%2Fwww.youtube.com%2Fsignin%3Faction_handle_signin%3Dtrue%26app%3Ddesktop%26hl%3Dru%26next%3Dhttps%253A%252F%252Fwww.youtube.com%252F&ec=65620&flowEntry=ServiceLogin&flowName=GlifWebSignIn&hl=ru&ifkv=Af_xneEq9uv5Q25YFpX_LHLrAVfg_vTq1qnBk1lKtXWIA2VDPXvnRE9uT5g-fnnTfVyPHPE1aRfTzw&passive=true&service=youtube&uilel=3");


			//DD.YTDriver.Navigate().GoToUrl("file:///C:/Users/73961/Desktop/fdsf.html");
			Thread.Sleep(5000);

			IWebElement ButtonGoogle = DD.YTDriver.FindElement(By.CssSelector("[data-oauthserver='https://accounts.google.com/o/oauth2/auth']"));
			ButtonGoogle.Click();


			Thread.Sleep(5000);

			IWebElement LoginArea = DD.YTDriver.FindElement(By.ClassName("zHQkBf"));
			LoginArea.SendKeys(DD.Log);

			IWebElement ButtonAreaLogin = DD.YTDriver.FindElement(By.ClassName("VfPpkd-LgbsSe-OWXEXe-k8QpJ"));
			ButtonAreaLogin.Click();

			Thread.Sleep(5000);


			IWebElement PassArea = DD.YTDriver.FindElement(By.ClassName("zHQkBf"));
			PassArea.SendKeys(DD.pas);

			IWebElement ButtonAreaPass = DD.YTDriver.FindElement(By.ClassName("VfPpkd-LgbsSe-OWXEXe-k8QpJ"));
			ButtonAreaPass.Click();
			Thread.Sleep(8000);



			try
			{
				IWebElement Error = DD.YTDriver.FindElement(By.ClassName("ahT6S"));
				DD.working = "No";
			}
			catch
			{
				try
				{
					IWebElement Error1 = DD.YTDriver.FindElement(By.ClassName("glT6eb"));
					DD.working = "No";
				}
				catch
				{
					DD.working = "Working";
				}
			}

			Thread.Sleep(500);
			SetTokens();

			return true;



		}






		private void button12_Click(object sender, EventArgs e)
		{
			ChangeAccount();
			// Начать спам
		}



		public void ChangeAccount()
		{
			//Войти в акк
			YTClass DD = null;

			foreach(YTClass YTC in YTDrivers)
			{
				if(YTC.Replies == 0)
				{
					DD = YTC;
					break;
				}
			}

			if(DD == null)
			{
				return;
			}


		
			if(DD.YTDriver != null)
			{
				DD.YTDriver.Quit();

			}

		

			
				DD.YTDriver = new ChromeDriver();
				DD.YTDriver.Navigate().GoToUrl("https://accounts.google.com/v3/signin/identifier?dsh=S2047072606%3A1683301289488608&continue=https%3A%2F%2Fwww.youtube.com%2Fsignin%3Faction_handle_signin%3Dtrue%26app%3Ddesktop%26hl%3Dru%26next%3Dhttps%253A%252F%252Fwww.youtube.com%252F&ec=65620&flowEntry=ServiceLogin&flowName=GlifWebSignIn&hl=ru&ifkv=Af_xneEq9uv5Q25YFpX_LHLrAVfg_vTq1qnBk1lKtXWIA2VDPXvnRE9uT5g-fnnTfVyPHPE1aRfTzw&passive=true&service=youtube&uilel=3");


				Thread.Sleep(5000);

				IWebElement LoginArea = DD.YTDriver.FindElement(By.ClassName("zHQkBf"));
				LoginArea.SendKeys(DD.Log);

				IWebElement ButtonAreaLogin = DD.YTDriver.FindElement(By.ClassName("VfPpkd-LgbsSe-OWXEXe-k8QpJ"));
				ButtonAreaLogin.Click();

				Thread.Sleep(5000);


				IWebElement PassArea = DD.YTDriver.FindElement(By.ClassName("zHQkBf"));
				PassArea.SendKeys(DD.pas);

				IWebElement ButtonAreaPass = DD.YTDriver.FindElement(By.ClassName("VfPpkd-LgbsSe-OWXEXe-k8QpJ"));
				ButtonAreaPass.Click();
				Thread.Sleep(5000);

			
			


			MakeSpam(DD);

		}

		public void MakeSpam(YTClass DD)
		{
			DD.YTDriver.Navigate().GoToUrl(VideoLink);

			Thread.Sleep(5000);

		/*	

			while(DD.Replies < 98)
			{

				((IJavaScriptExecutor)DD.YTDriver).ExecuteScript("window.scrollTo(0, 999999999999999999999)");


				Thread.Sleep(2000);
			}
*/
			



			/*File.WriteAllText(@"Discord/" + _ServerName + ".txt", "");

			using (StreamWriter writer = new StreamWriter(@"Discord/" + _ServerName + ".txt"))
			{
				foreach (string line in DiscordMembers)
				{
					writer.WriteLine(line);
				}
			}*/
		}


		private void button10_Click(object sender, EventArgs e)
		{
			// Сделать токены
			try
			{
				YouTubeTokens = new List<string>();


				using (StreamReader reader = new StreamReader(@"YouTube/Tokens.txt"))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						string logpas = line.Split("|")[1];
						string result = logpas.Replace("\"", "");
						result = logpas.Replace(" ", "");
						result += ":Kirilllachaev:@lachaev:Yes:0";
						YouTubeTokens.Add(result);
					}
				}






				File.WriteAllText(@"YouTube/Tokens.txt", "");

				using (StreamWriter writer = new StreamWriter(@"YouTube/Tokens.txt"))
				{
					foreach (string line in YouTubeTokens)
					{

						writer.WriteLine(line.Replace("\"", "").Replace(" ", ""));
					}
				}
			}
			catch
			{
				MessageBox.Show("Не подходящий формат");
			}


		}

		#endregion




		#region Discord

		public List<string> DiscordMembers;
		public List<string> DiscordTokens;

		public string _ServerName = "";

		string MessageText = "";
		string MessageText2 = "";

		public List<DiscordDriver> DiscordDrivers;


		public int Done;


	


		private void button7_Click(object sender, EventArgs e)
		{

			MessageText = File.ReadAllText(@"Discord/Message.txt");
			MessageText2 = File.ReadAllText(@"Discord/Message2.txt");
		

			_ServerName = textBox3.Text;

			getMembers();

			DiscordTokens = new List<string>();
			using (StreamReader reader = new StreamReader(@"Discord/Tokens.txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					DiscordTokens.Add(line);
				}
			}

			MessageBox.Show("Загружено " + DiscordMembers.Count + " ссылок");

			DiscordDrivers = new List<DiscordDriver>();


			for(int i = 0; i < DiscordTokens.Count; i++)
			{
				Thread tt = new Thread(() => MakeDD());
				tt.Start();

				Thread.Sleep(1000);


			}

			

			Thread t = new Thread(() => MessageBox.Show("Vse sozdani"));
			t.Start();


		}

		private void button8_Click(object sender, EventArgs e)
		{
			
			// Начать спам

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


		public void getMembers()
		{
			DiscordMembers = new List<string>();
			Done = 0;
			using (StreamReader reader = new StreamReader(@"Discord/" + _ServerName + ".txt"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					DiscordMembers.Add(line);
					if (line.Contains("☻"))
					{
						Done += 1;
					}
				}
			}
		
		}


		public void MakeDD()
		{
			int _num = DiscordDrivers.Count;
			DiscordDriver DD = new DiscordDriver();
			DiscordDrivers.Add(DD);


			if (_num != 0)
			{
				Thread.Sleep(30000 * _num); 
			}


			// Дать логин пароль
		
			DD.Log = DiscordTokens[_num].Split(':')[0];
			DD.pas = DiscordTokens[_num].Split(':')[1];

			try
			{
				MakeLogin(DD, 1);
				Thread.Sleep(5000);
			}
			catch
			{
				try
				{
					MakeLogin(DD, 2);
					Thread.Sleep(10000);
				}
				catch
				{
					MakeLogin(DD, 3);
					Thread.Sleep(15000);
				}
			}
		

			DD.DiscrodSpamThread = new Thread(() => DiscordSpam(DD));
			DD.DiscrodSpamThread.Start();


		}

		public void MakeLogin(DiscordDriver DD, int mult) 
		{
			DD.DiscrodDriver = new ChromeDriver();
			DD.DiscrodDriver.Navigate().GoToUrl("https://discord.com/login");

			Thread.Sleep(5000 * mult);


			IWebElement LoginArea = DD.DiscrodDriver.FindElement(By.CssSelector("[aria-label='Адрес электронной почты или номер телефона']"));
			IWebElement PassArea = DD.DiscrodDriver.FindElement(By.CssSelector("[aria-label='Пароль']"));
			LoginArea.SendKeys(DD.Log);
			PassArea.SendKeys(DD.pas);

			Thread.Sleep(2000);

			IWebElement ButtonArea = DD.DiscrodDriver.FindElement(By.ClassName("button-1cRKG6"));

			ButtonArea.Click();
		}

		public void DiscordSpam(DiscordDriver drive)
		{


			for (int i = 1; i < DiscordMembers.Count; i++)
			{
				getMembers();


				if (DiscordMembers[i].Contains("☻"))
				{
					
				}
				else
				{
					try
					{
						MakeSpamMessage(drive, i, 1);
					}
					catch
					{
						try
						{
							Thread.Sleep(5000);
							MakeSpamMessage(drive, i, 2);
						}
						catch
						{
							Thread.Sleep(5000);
							MakeSpamMessage(drive, i, 3);
						}
						
					}
					
				}

				

				


			}
		}

		public void MakeSpamMessage(DiscordDriver drive, int i, int mult)
		{
			drive.DiscrodDriver.Navigate().GoToUrl("https://discord.com/users/" + DiscordMembers[i]);
			Thread.Sleep(5000* mult);


			IWebElement ButtonParent = drive.DiscrodDriver.FindElement(By.ClassName("relationshipButtons-3ByBpj"));
			IWebElement ButtonArea = ButtonParent.FindElement(By.CssSelector("[role = 'button']"));

			ButtonArea.Click();
			Thread.Sleep(1000);


			IWebElement ButtonMessage = drive.DiscrodDriver.FindElement(By.Id("user-profile-actions-user-message"));
			ButtonMessage.Click();

			Thread.Sleep(5000 * mult);

			IWebElement TextParent = drive.DiscrodDriver.FindElement(By.ClassName("emptyText-1o0WH_"));
			IWebElement TextArea = TextParent.FindElement(By.CssSelector("span"));

			if (i % 2 == 0)
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



			drive.Seconds = 661;
			Thread.Sleep(661000);
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
			//driver.Quit();
			foreach(DiscordDriver DD in DiscordDrivers)
			{
				DD.DiscrodDriver.Quit();
			}
			Application.Exit();
		
		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}


		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		#endregion

		private void timer1_Tick(object sender, EventArgs e)
		{

			/*
			if(DiscordDrivers != null)
			{
				for (int i = 0; i < DiscordDrivers.Count; i++)
				{
					if (DiscordDrivers[i].Seconds > 0)
					{
						DiscordDrivers[i].Seconds -= 1;

						string times = "";
						foreach (DiscordDriver DD in DiscordDrivers)
						{
							times += DD.Seconds + "    /     ";
						}
						label6.Text = times;


					}
				}
			}

			label5.Text = Done.ToString();
			*/

		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{

		}

		
	}
}
