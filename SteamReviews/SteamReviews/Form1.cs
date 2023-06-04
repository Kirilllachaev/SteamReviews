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

		Thread SpamThread = null;

		public int pageNumber = 0;

		public List<string> Alllinks = new List<string>();

		public string selectedLanguage = "all";


		public string gameName = "";
		public string gameLink = "";



		public string linksFile = "";

		public int allLinkCount = 0;
		public GamePage gamePage;



		#region Steam

		private void button1_Click_1(object sender, EventArgs e)
		{

			if (driver != null)
				driver.Quit();

			driver = new ChromeDriver();
			driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");

			gameName = textBox2.Text;
			gameLink = textBox1.Text;

			//
			getReviews();



		}

		public void getReviews()
		{

			GamePage GP = new GamePage();
			GP.gameName = gameName;
			GP.ReviewLangs = new List<ReviewLang>();
			int count = 0;
			int spammed = 0;
			int closed = 0;

			try
			{
				string[] txtFiles = Directory.GetFiles(@"Steam/" + gameName, "*.txt");

				foreach (string filePath in txtFiles)
				{
					string fileName = Path.GetFileName(filePath);
					ReviewLang RL = new ReviewLang();
					RL.lang = fileName.Split('_')[1].Split('.')[0];
					RL.Links = new List<string>();

					string[] lines = File.ReadAllLines(filePath);
	
					foreach (string line in lines)
					{				
						
						count += 1;
						if (line.Contains("?donsk"))
						{
							spammed += 1;
						}
						if (line.Contains("?closed"))
						{
							closed += 1;
						}
						RL.Links.Add(line);
					}
					GP.ReviewLangs.Add(RL);

				}
				label4.Text = count.ToString();
				label5.Text = spammed.ToString();
				label6.Text = closed.ToString();
			}
			catch (DirectoryNotFoundException)
			{
				Console.WriteLine("Указанная папка не найдена.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Произошла ошибка: " + ex.Message);
			}
			gamePage = GP;




		}



		public void FreshCounts()
		{
			int count = 0;
			int spammed = 0;
			int closed = 0;
			int deleted = 0;
			foreach (ReviewLang RL in gamePage.ReviewLangs)
			{
				foreach (string L in RL.Links)
				{
					count += 1;
					if (L.Contains("?donsk"))
					{
						spammed += 1;
					}
					if (L.Contains("?closed"))
					{
						closed += 1;
					}
					if (L.Contains("?donskdel"))
					{
						deleted += 1;
					}
				}
			}
			label4.Text = count.ToString();
			label5.Text = spammed.ToString();
			label6.Text = closed.ToString();
			label7.Text = deleted.ToString();

		}




		private void button2_Click(object sender, EventArgs e)
		{

		


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

		bool spamstarted = false;

		private void button5_Click(object sender, EventArgs e)
		{
			if (spamstarted)
				return;

			spamstarted = true;

			//SpamThread = new Thread(() => SteamSpaming());
			//SpamThread.Start();
			SteamSpaming();


		}

		Dictionary<string, string> translations = new Dictionary<string, string>()
		{
			{"italian", "Ciao!\nStiamo sviluppando un gioco simile.\nForse ti piacerà anche :)\nSe sì, aggiungilo alla tua lista dei desideri e unisciti al nostro server Discord!\nGrazie!"},
			{"polish", "Cześć!\nRozwijamy podobną grę.\nByć może również Ci się spodoba :)\nJeśli tak, dodaj ją do swojej listy życzeń i dołącz do naszego serwera Discord!\nDziękujemy!"},
			{"schinese", "你好！\n我们正在开发类似的游戏。\n也许你也会喜欢它 :)\n如果是这样，请将其添加到您的愿望清单中并加入我们的Discord服务器！\n谢谢！"},
			{"tchinese", "你好！\n我們正在開發類似的遊戲。\n也許你也會喜歡它 :)\n如果是這樣，請將其添加到您的願望清單中並加入我們的Discord伺服器！\n謝謝！"},
			{"japanese", "こんにちは！\n私たちは似たようなゲームを開発しています。\nもしかしたら気に入るかもしれません :)\nもしそうなら、ぜひウィッシュリストに追加して、私たちのDiscordサーバーに参加してください！\nありがとうございます！"},
			{"koreana", "안녕하세요!\n비슷한 게임을 개발하고 있습니다.\n아마도 마음에 드실 것입니다 :)\n그렇다면, 찜 목록에 추가하고 Discord 서버에 참여해주세요!\n감사합니다!"},
			{"thai", "สวัสดีครับ!\nเรากำลังพัฒนาเกมที่คล้ายกันนี้\nอาจจะชอบกันเช่นกัน :)\nถ้าใช่ กรุณาเพิ่มลงในรายการที่คุณต้องการและเข้าร่วมเซิร์ฟเวอร์ Discord ของเรา!\nขอบคุณครับ!"},
			{"bulgarian", "Здравейте!\nРазработваме подобна игра.\nВозможно, ще ви хареса :)\nАко да, моля, добавете я във вашите желания и се присъединете към нашия Discord сървър!\nБлагодарим ви!"},
			{"czech", "Ahoj!\nVyvíjíme podobnou hru.\nMožná se vám bude líbit taky :)\nPokud ano, přidejte ji do svého seznamu přání a připojte se k našemu serveru Discord!\nDěkujeme!"},
			{"danish", "Hej!\nVi udvikler et lignende spil.\nMåske vil du også kunne lide det :)\nHvis ja, tilføj det til din ønskeliste og join vores Discord-server!\nTak!"},
			{"german", "Hallo!\nWir entwickeln ein ähnliches Spiel.\nVielleicht gefällt es Ihnen auch :)\nWenn ja, fügen Sie es bitte zu Ihrer Wunschliste hinzu und treten Sie unserem Discord-Server bei!\nVielen Dank!"},
			{"english", "Hello!\nWe are developing a similar game.\nMaybe you will like it too :)\nIf so, please add it to your wishlist and join our Discord server!\nThank you!"},
			{"spanish", "¡Hola!\nEstamos desarrollando un juego similar.\nTal vez también te guste :)\nSi es así, por favor, ¡añádelo a tu lista de deseos y únete a nuestro servidor Discord!\n¡Gracias!"},
			{"latam", "¡Hola!\nEstamos desarrollando un juego similar.\nTal vez también te guste :)\nSi es así, por favor, ¡añádelo a tu lista de deseos y únete a nuestro servidor Discord!\n¡Gracias!"},
			{"greek", "Γεια σου!\nΑναπτύσσουμε ένα παρόμοιο παιχνίδι.\nΊσως σου αρέσει επίσης :)\nΑν ναι, προσθέστε το στη λίστα επιθυμιών σας και ελάτε στον διακομιστή Discord μας!\nΣας ευχαριστώ!"},
			{"french", "Salut !\nNous développons un jeu similaire.\nPeut-être que ça vous plaira aussi :)\nSi oui, ajoutez-le à votre liste de souhaits et rejoignez notre serveur Discord !\nMerci !"},
			{"hungarian", "Szia!\nHasonló játékot fejlesztünk.\nLehet, hogy neked is tetszeni fog :)\nHa igen, add hozzá a kívánságlistádhoz, és csatlakozz a Discord szerverünkhöz!\nKöszönjük!"},
			{"dutch", "Hallo!\nWe zijn bezig met de ontwikkeling van een vergelijkbare game.\nMisschien vind je het ook leuk :)\nVoeg het toe aan je verlanglijst en sluit je aan bij onze Discord-server!\nBedankt!"},
			{"norwegian", "Hei!\nVi utvikler et lignende spill.\nKanskje du også vil like det :)\nHvis ja, vennligst legg det til i ønskelisten din og bli med på vår Discord-server!\nTakk!"},
			{"portuguese", "Olá!\nEstamos desenvolvendo um jogo semelhante.\nTalvez você também goste :)\nSe sim, adicione-o à sua lista de desejos e junte-se ao nosso servidor Discord!\nObrigado!"},
			{"brazilian", "Olá!\nEstamos desenvolvendo um jogo similar.\nTalvez você também goste :)\nSe sim, adicione-o à sua lista de desejos e junte-se ao nosso servidor Discord!\nObrigado!"},
			{"romanian", "Salut!\nDezvoltăm un joc similar.\nPoate îți va plăcea și ție :)\nDacă da, te rugăm să-l adaugi în lista ta de dorințe și să te alături serverului nostru Discord!\nMulțumim!"},
			{"russian", "Привет!\nМы разрабатываем похожую игру.\nВозможно, она вам тоже понравится :)\nЕсли да, добавьте её в свой список желаемого и присоединяйтесь к нашему серверу Discord!\nСпасибо!"},
			{"finnish", "Hei!\nKehitämme samankaltaista peliä.\nSaattaisit pitää siitä myös :)\nJos näin on, lisää se toivelistaasi ja liity Discord-palvelimeemme!\nKiitos!"},
			{"swedish", "Hej!\nVi utvecklar ett liknande spel.\nKanske kommer du också tycka om det :)\nOm så är fallet, lägg till det på din önskelista och anslut till vår Discord-server!\nTack!"},
			{"turkish", "Merhaba!\nBenzer bir oyun geliştiriyoruz.\nBelki de size de hoş gelebilir :)\nEğer öyleyse, lütfen istek listesine ekleyin ve Discord sunucumuza katılın!\nTeşekkür ederiz!"},
			{"vietnamese", "Xin chào!\nChúng tôi đang phát triển một trò chơi tương tự.\nCó thể bạn cũng thích nó :)\nNếu vậy, hãy thêm nó vào danh sách mong muốn của bạn và tham gia máy chủ Discord của chúng tôi!\nCảm ơn bạn!"},
			{"ukrainian", "Привіт!\nМи розробляємо схожу гру.\nМожливо, вона вам також сподобається :)\nЯкщо так, додайте її до свого списку бажань і приєднуйтеся до нашого серверу Discord!\nДякуємо!"}
		};


		public void SteamSpaming()
		{
			if (gamePage.ReviewLangs.Count > 0)
			{
				foreach (ReviewLang RL in gamePage.ReviewLangs)
				{
					for (int i = 0; i < RL.Links.Count; i++)
					{
						string L = RL.Links[i];
						if (!L.Contains("?donsk") && !L.Contains("?closed"))
						{
							driver.Navigate().GoToUrl(L);

							Thread.Sleep(1000);

							try
							{
								IWebElement textarea = driver.FindElement(By.CssSelector("[placeholder='Оставить комментарий']"));
								string text = translations[RL.lang];
								textarea.SendKeys(text);
								Thread.Sleep(500);
								IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
								greenButton.Click();

								RL.Links[RL.Links.IndexOf(L)] += "?donsk";


							}
							catch
							{
								RL.Links[RL.Links.IndexOf(L)] += "?closed";
							}

							File.WriteAllLines(@"Steam/" + gameName + "/" + gameName + "_" + RL.lang + ".txt", RL.Links);
							FreshCounts();



						}

					}




				}
			}
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

				

				((IJavaScriptExecutor)myDriver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

				Thread.Sleep(100);

				IWebElement end = myDriver.FindElement(By.ClassName("apphub_NoMoreContent"));


				//IWebElement page = myDriver.FindElement(By.Id("page" + pageNumber.ToString()));
			


				if (end.GetAttribute("style").Contains("opacity: 1;"))
				{

					ReadOnlyCollection<IWebElement> childElements = myDriver.FindElements(By.ClassName("apphub_Card"));
					IWebElement[] cards = childElements.ToArray();

					foreach (IWebElement E in cards)
					{
						string link = E.GetAttribute("data-modal-content-url");
						if (!Alllinks.Contains(link))
						{
							Alllinks.Add(link);
						}

					}


					// Законченож
					isScrolling = false;

					if (One)
					{
						MakeOutput(gameName, _selectedLanguage);
						return;
					}
				}



				pageNumber += 1;
				

				

				Thread.Sleep(1000);

				

			}
		}

		private void button6_Click_1(object sender, EventArgs e)
		{
			// удалить коммы
			SteamDeleting();
		}

		public void SteamDeleting()
		{
			if (gamePage.ReviewLangs.Count > 0)
			{
				foreach (ReviewLang RL in gamePage.ReviewLangs)
				{
					for (int i = 0; i < RL.Links.Count; i++)
					{
						string L = RL.Links[i];
						if (L.Contains("?donsk") && !L.Contains("?donskdel"))
						{
							driver.Navigate().GoToUrl(L);

							Thread.Sleep(1000);

							IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName("commentthread_comment"));

							// Создание экземпляра класса Actions
							Actions actions = new Actions(driver);

							// Применение состояния :hover к каждому найденному элементу
							foreach (IWebElement element1 in elements)
							{
								actions.MoveToElement(element1).Perform();
							}

							try
							{
								IWebElement element = driver.FindElement(By.CssSelector("[data-tooltip-text='Удалить']"));
								element.Click();
								RL.Links[RL.Links.IndexOf(L)] += "del";
							}
							catch
							{
								try
								{
									IWebElement element = driver.FindElement(By.CssSelector("[data-tooltip-text='Удалить']"));
									element.Click();
									RL.Links[RL.Links.IndexOf(L)] += "del";
								}
								catch
								{

								}
							}


							

							File.WriteAllLines(@"Steam/" + gameName + "/" + gameName + "_" + RL.lang + ".txt", RL.Links);
							FreshCounts();



						}

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
				System.IO.Directory.CreateDirectory(@"Steam/" + _gameName);
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
					/*while ((line = reader.ReadLine()) != null)
					{
						string logpas = line.Split("|")[1];
						string result = logpas.Replace("\"", "");
						result = logpas.Replace(" ", "");
						result += ":Kirilllachaev:Kirilllachaev:No:0";
						YouTubeTokens.Add(result);
					}*/

					while ((line = reader.ReadLine()) != null)
					{
						string log = line.Split(":")[0];
						string pas = line.Split(":")[1];
						string result = log + ":" + pas;
						result = result.Replace(" ", "");
						result += ":Kirilllachaev:Kirilllachaev:No:0";
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
			driver.Quit();
		
			Application.Exit();
		
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

		}

		private void label4_Click(object sender, EventArgs e)
		{

		}

		private void label5_Click_1(object sender, EventArgs e)
		{

		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}
	}


	public class ReviewLang
	{
		public string lang;
		public List<string> Links = new List<string>();
	}
	public class GamePage
	{
		public string gameName;
		public List<ReviewLang> ReviewLangs = new List<ReviewLang>();
	}
}
