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



			string[] subdirectories = Directory.GetDirectories(@"Steam/");

			foreach(string Dir in subdirectories)
			{
				string mdir = Dir.Split("/")[1];
				
				int count = 0;
				int spammed = 0;
				int closed = 0;

				try
				{
					string[] txtFiles = Directory.GetFiles(@"Steam/" + mdir, "*.txt");

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

					}
					
				}
				catch
				{

				}


				string[] row = { mdir, count.ToString(), (spammed+closed).ToString(),spammed.ToString(), closed.ToString() };
				dataGridView1.Rows.Add(row);
			}

		}

		ChromeDriver driver = null;
		bool isScrolling = false;
		bool isCollecting = false;
		Thread ScrollingThread = null;
		Thread MultipleLangsThread = null;

		Thread SpamThread = null;

		public int pageNumber = 0;
		public int SpamerNumber = 0;

		public List<string> Alllinks = new List<string>();

		public string selectedLanguage = "all";

		public string[] SpamNames = { "Deep Dive Duet", "Long Way", "Don Duality", "Sunkenland", "Absence of Light" };
		public string[] SpamLinks = { "https://store.steampowered.com/app/2712970/Deep_Dive_Duet/", "https://store.steampowered.com/app/2455070/Long_Way/", "https://store.steampowered.com/app/2247570/Don_Duality/", "https://store.steampowered.com/app/2080690/Sunkenland/", "https://store.steampowered.com/app/2536710/Absence_of_Light/" };


		public string gameName = "";
		public string gameLink = "";
		public string steamacc = "";


		public string linksFile = "";

		public int allLinkCount = 0;
		public GamePage gamePage;

		public bool ContinueSbor = false;

		public string DriverProxy;
		public bool UseProxy;





		#region Steam

		

		private void button1_Click_1(object sender, EventArgs e)
		{

			if (driver != null)
				driver.Quit();


			SpamerNumber = Int32.Parse(textBox4.Text);

			DriverProxy = textBox5.Text;
			UseProxy = checkBox2.Checked;

			if (UseProxy)
			{
				ChromeOptions options = new ChromeOptions();
				Proxy proxy = new Proxy();
				proxy.Kind = ProxyKind.Manual;
				proxy.HttpProxy = DriverProxy;
				proxy.SslProxy = DriverProxy;

				// Назначение прокси-настроек объекту опций
				options.Proxy = proxy;

				driver = new ChromeDriver(options);
			}
			else
			{
				driver = new ChromeDriver();
			}
			
			driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/");

			gameName = textBox2.Text;
			gameLink = textBox1.Text;
			steamacc = textBox3.Text;

			ContinueSbor = checkBox1.Checked;

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

		Dictionary<string, string> messages = new Dictionary<string, string>
		{
			{ "italian", "Ciao!\nStiamo facendo un gioco simile e potrebbe piacerti!\nPer favore, dai un'occhiata alla pagina del nostro gioco.\nSe ti piace, aggiungi il gioco alla tua lista dei desideri, ciò ci aiuterà nella promozione!\nScusaci per il disturbo e grazie mille!\n\nGioco:" },
			{ "polish", "Cześć!\nRobimy podobną grę, która może ci się spodobać!\nProszę, sprawdź stronę naszej gry.\nJeśli ci się podoba, dodaj ją do listy życzeń, pomoże nam to w promocji!\nPrzepraszamy za kłopot i dziękujemy!\n\nGra:" },
			{ "schinese", "你好！\n我们正在制作一款类似的游戏，也许你会喜欢！\n请查看我们游戏的页面。\n如果喜欢，请将游戏添加到愿望列表，这将有助于推广！\n为打扰你感到抱歉，非常感谢！\n\n游戏：" },
			{ "tchinese", "你好！\n我們正在製作一款類似的遊戲，也許你會喜歡！\n請查看我們遊戲的頁面。\n如果喜歡，請將遊戲添加到願望列表，這將有助於推廣！\n為打擾你感到抱歉，非常感謝！\n\n遊戲：" },
			{ "japanese", "こんにちは！\n似たようなゲームを作っていて、もしかしたら気に入っていただけるかもしれません！\n当社のゲームのページをご覧ください。\n気に入った場合は、ゲームをウィッシュリストに追加していただけると、プロモーションに役立ちます！\nご迷惑をおかけして申し訳ありません、どうもありがとうございます！\n\nゲーム：" },
			{ "koreana", "안녕하세요!\n우리는 비슷한 게임을 만들고 있어, 아마도 마음에 드실 거에요!\n저희 게임 페이지를 확인해보세요.\n마음에 들면 게임을 위시리스트에 추가해주세요, 이는 홍보에 도움이 될 거예요!\n불편을 끼쳐드려 죄송하고 감사합니다!\n\n게임:" },
			{ "thai", "สวัสดีครับ/ค่ะ!\nเรากำลังทำเกมที่คล้ายกันและอาจจะถูกใจคุณ!\nโปรดดูหน้าเกมของเรา\nหากคุณชอบ กรุณาเพิ่มเกมลงในรายการที่ต้องการ เพื่อช่วยสนับสนุนการโปรโมท!\nขออภัยในความไม่สะดวกและขอบคุณมากครับ/ค่ะ!\n\nเกม:" },
			{ "bulgarian", "Здравейте!\nНие правим подобна игра и вероятно ще ви хареса!\nМоля, разгледайте страницата на нашата игра.\nАко ви хареса, добавете я в списъка с желания, това ще помогне за промоцията!\nИзвиняваме се за притеснението и ви благодарим много!\n\nИгра:" },
			{ "czech", "Ahoj!\nDěláme podobnou hru a možná se vám bude líbit!\nProsím, podívejte se na stránku naší hry.\nPokud se vám líbí, přidejte hru do seznamu přání, pomůže nám to s propagací!\nOmlouváme se za obtěžování a děkujeme vám moc!\n\nHra:" },
			{ "danish", "Hej!\nVi laver et lignende spil, og måske vil du også kunne lide det!\nTag et kig på vores spils side, hvis du vil.\nHvis du kan lide det, så tilføj spillet til din ønskeliste; det vil hjælpe med at fremme det!\nUndskyld for besværet og tak skal du have!\n\nSpil:" },
			{ "german", "Hallo!\nWir machen ein ähnliches Spiel und vielleicht gefällt es dir auch!\nBitte schau dir die Seite unseres Spiels an.\nWenn es dir gefällt, füge das Spiel bitte deiner Wunschliste hinzu, das hilft bei der Promotion!\nEntschuldige die Störung und vielen Dank!\n\nSpiel:" },
			{ "english", "Hello!\nWe are making a similar game, and perhaps you will like it too!\nPlease check out our game's page.\nIf you like it, please add the game to your wishlist; it will help in promotion!\nSorry for the inconvenience, and thank you so much!\n\nGame:" },
			{ "spanish", "¡Hola!\nEstamos haciendo un juego similar y tal vez te guste también.\nPor favor, visita la página de nuestro juego.\nSi te gusta, añade el juego a tu lista de deseos, ¡ayudará en la promoción!\nDisculpa las molestias y ¡muchas gracias!\n\nJuego:" },
			{ "latam", "¡Hola!\nEstamos haciendo un juego similar y quizás te guste también.\nPor favor, visita la página de nuestro juego.\nSi te gusta, añade el juego a tu lista de deseos, ¡ayudará en la promoción!\nDisculpa las molestias y ¡muchas gracias!\n\nJuego:" },
			{ "greek", "Γεια σας!\nΦτιάχνουμε ένα παρόμοιο παιχνίδι και ίσως σας αρέσει επίσης!\nΠαρακαλώ, ελέγξτε τη σελίδα του παιχνιδιού μας.\nΑν σας αρέσει, παρακαλούμε προσθέστε το παιχνίδι στη λίστα επιθυμιών σας, θα βοηθήσει στην προώθηση!\nΣυγγνώμη για την ταλαιπωρία και ευχαριστώ πολύ!\n\nΠαιχνίδι:" },
			{ "french", "Salut !\nNous faisons un jeu similaire et peut-être qu'il te plaira aussi !\nMerci de consulter la page de notre jeu.\nSi tu aimes, ajoute le jeu à ta liste de souhaits, cela aidera à la promotion !\nDésolé pour le dérangement et merci beaucoup !\n\nJeu :" },
			{ "hungarian", "Helló!\nHasonló játékot készítünk, és talán neked is tetszeni fog!\nKérlek, nézd meg a játékunk oldalát.\nHa tetszik, add hozzá a játékot a kívánságlistádhoz, ez segít a promócióban!\nElnézést a kellemetlenségért és köszönjük szépen!\n\nJáték:" },
			{ "dutch", "Hallo!\nWe maken een vergelijkbaar spel en misschien vind je het ook leuk!\nBekijk alsjeblieft de pagina van ons spel.\nAls het je bevalt, voeg het spel dan toe aan je verlanglijstje; het zal helpen bij de promotie!\nSorry voor het ongemak en hartelijk dank!\n\nSpel:" },
			{ "norwegian", "Hei!\nVi lager et lignende spill, og kanskje vil du også like det!\nVennligst sjekk ut spill-siden vår.\nHvis du liker det, vennligst legg til spillet på ønskelisten din; det vil hjelpe med promoteringen!\nUnnskyld for bryderiet og tusen takk!\n\nSpill:" },
			{ "portuguese", "Oi!\nEstamos fazendo um jogo semelhante e talvez você também goste!\nPor favor, confira a página do nosso jogo.\nSe gostar, adicione o jogo à sua lista de desejos; isso ajudará na promoção!\nDesculpe pelo incômodo e muito obrigado!\n\nJogo:" },
			{ "brazilian", "Oi!\nEstamos fazendo um jogo semelhante e talvez você também goste!\nPor favor, confira a página do nosso jogo.\nSe gostar, adicione o jogo à sua lista de desejos; isso ajudará na promoção!\nDesculpe pelo incômodo e muito obrigado!\n\nJogo:" },
			{ "romanian", "Salut!\nFacem un joc similar și poate îți va plăcea și ție!\nTe rog să verifici pagina jocului nostru.\nDacă îți place, te rog adaugă jocul în lista ta de dorințe, asta va ajuta la promovare!\nNe pare rău pentru inconveniențe și îți mulțumim mult!\n\nJoc:" },
			{ "russian", "Привет!\nМы делаем похожую игру и, возможно, она тебе тоже понравится!\nПожалуйста, посмотри страницу нашей игры.\nЕсли понравится – пожалуйста, добавь игру в список желаемого, это поможет в продвижении!\nИзвини за беспокойство и огромное тебе спасибо!\n\nИгра:" },
			{ "finnish", "Hei!\nTeemme samankaltaista peliä, ja ehkä pidät siitä myös!\nKatsohan pelimme sivu.\nJos se miellyttää, lisää peli toivelistallesi, se auttaa promootiossa!\nAnteeksi häiriöstä ja kiitos paljon!\n\nPeli:" },
			{ "swedish", "Hej!\nVi gör ett liknande spel, och kanske gillar du det också!\nVänligen kolla in vår spelesida.\nOm du gillar det, lägg till spelet på din önskelista; det hjälper till med marknadsföringen!\nFörlåt för besväret och tack så mycket!\n\nSpel:" },
			{ "turkish", "Merhaba!\nBenzer bir oyun yapıyoruz ve belki de sana da hoş gelebilir!\nLütfen oyunumuzun sayfasını kontrol et.\nEğer beğenirsen, lütfen oyunu dilek listene ekle; bu tanıtıma yardımcı olur!\nRahatsızlık için özür dileriz ve çok teşekkür ederiz!\n\nOyun:" },
			{ "vietnamese", "Chào bạn!\nChúng tôi đang làm một trò chơi tương tự và có thể bạn cũng sẽ thích nó!\nHãy xem trang trò chơi của chúng tôi.\nNếu bạn thích – vui lòng thêm trò chơi vào danh sách mong muốn, điều này sẽ giúp quảng bá!\nXin lỗi vì sự phiền phức và cảm ơn bạn rất nhiều!\n\nTrò chơi:" },
			{ "ukrainian", "Привіт!\nМи робимо схожу гру і, можливо, вона тобі також сподобається!\nБудь ласка, переглянь сторінку нашої гри.\nЯкщо сподобається – будь ласка, додай гру до списку бажаного, це допоможе у продвиженні!\nВибач за непокій і велике тобі спасибі!\n\nГра:" }
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
								string text = messages[RL.lang];
								

								/*Random random = new Random();
								int randomIndex = random.Next(6 + ((SpamerNumber+1) * 2)); // 6 options

								switch (randomIndex)
								{
									case 0:
										text = text.Replace(".", ";");
										break;
									case 1:
										text = text.Replace(".", ",");
										break;
									case 2:
										text = text.Replace(",", ";");
										break;
									case 3:
										text = text.Replace(",", ".");
										break;
									case 4:
										text = text.Replace("!", "");
										break;
									case 5:
										text = text.Replace("!", "!!");
										break;
								}*/

								text += SpamNames[SpamerNumber];
								text += "\n";
								text += SpamLinks[SpamerNumber];

								textarea.SendKeys(text);
								Thread.Sleep(1000);
								IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
								greenButton.Click();


								Thread.Sleep(500);
								IWebElement errorElement = driver.FindElement(By.ClassName("commentthread_entry_error"));
								string errorText = errorElement.Text;
								if (errorText == "Простите, что-то пошло не так: вы слишком часто отправляете сообщения, передохните немного")
								{

									MessageBox.Show("Сообщение не отправлено. Аккаунт или Прокси в муте", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);                               }
								else
								{
								
									RL.Links[RL.Links.IndexOf(L)] += "?donsk";
								}

						
								


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

				bool endall = true;

				if (ContinueSbor)
				{
					bool haslang = false;
					foreach (ReviewLang RL in gamePage.ReviewLangs)
					{
						if (RL.lang == CselectedLanguage)
						{
							haslang = true;
							
						}
					}

					if (haslang)
					{
						endall = false;
					}

				}

				if (endall)
				{
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
				}
				



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


		private void button7_Click(object sender, EventArgs e)
		{
			steamacc = textBox3.Text;
			// Собрать коммы

			SteamSpamCollecting();
		}

		public void SteamSpamCollecting()
		{
			List<string> links = new List<string>();

			bool dabs = true;
			// Цикл для повторения действий

			try
			{
				while (dabs)
				{
					// Найти все элементы с классом comment_item_title
					IReadOnlyCollection<IWebElement> titles = driver.FindElements(By.ClassName("comment_item_title"));

					// Обработка каждого элемента
					foreach (IWebElement title in titles)
					{
						// Найти ссылку внутри элемента <a>
						IWebElement linkElement = title.FindElement(By.TagName("a"));

						// Получить URL ссылки
						string link = linkElement.GetAttribute("href");

						// Добавить ссылку в массив
						links.Add(link);
					}

					// Найти все элементы с классом pagebtn
					IReadOnlyCollection<IWebElement> pageButtons = driver.FindElements(By.ClassName("pagebtn"));

					// Проверить, содержит ли второй элемент класс disabled
					if (pageButtons.ElementAt(1).GetAttribute("class").Contains("disabled"))
					{
						// Если второй элемент содержит класс disabled, выйти из цикла

						System.IO.File.WriteAllLines(@"Steam/" + steamacc + ".txt", links);


						dabs = false;
						break;

					}

					// Нажать на второй элемент
					IWebElement secondPageButton = pageButtons.ElementAt(1);
					secondPageButton.Click();




				}
			}
			catch
			{

				System.IO.File.WriteAllLines(@"Steam/" + steamacc + ".txt", links);
			}
			
		}

		private void button6_Click_1(object sender, EventArgs e)
		{
			// удалить коммы
			SteamDeleting();
		}

		public void SteamDeleting()
		{

			string[] lines = File.ReadAllLines(@"Steam/" + steamacc + ".txt");
			string[] linecop = lines;

			for(int i = 0; i < lines.Length; i++)
			{
				
				string L = lines[i];
				if (!L.Contains("?del"))
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
						Thread.Sleep(500);
						
						IWebElement bdiElement = element1.FindElement(By.TagName("bdi"));
						if (bdiElement.Text == steamacc)
						{
							IWebElement element = driver.FindElement(By.CssSelector("[data-tooltip-text='Удалить']"));
							element.Click();
						
							Thread.Sleep(100);
							break;
						}

						
					}

					linecop[i] += "?del";




					File.WriteAllLines(@"Steam/" + steamacc + ".txt", linecop);
				}

				
			

			}


		}


		public void MakeOutput(string _gameName, string _selectedLanguage)
		{

			if (System.IO.Directory.Exists(@"Steam/" + _gameName))
			{
				System.IO.File.WriteAllLines(@"Steam/" + _gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
			}
			else
			{
				System.IO.Directory.CreateDirectory(@"Steam/" + _gameName);
				System.IO.File.WriteAllLines(@"Steam/" + _gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
			}

			allLinkCount += Alllinks.Count;

			Console.WriteLine("Сбор языка " + _selectedLanguage + " завершён. Было собрано " + Alllinks.Count + " ссылок");
			//Thread t = new Thread(() => MessageBox.Show("Сбор языка " + _selectedLanguage + " завершён. Было собрано " + Alllinks.Count + " ссылок", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning));
			//t.Start();

			Thread.Sleep(500);

			pageNumber = 0;
			Alllinks.Clear();

		}





		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedLanguage = comboBox1.Text;
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
			if(driver != null)
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

		private void textBox3_TextChanged_1(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void textBox5_TextChanged(object sender, EventArgs e)
		{

		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
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
