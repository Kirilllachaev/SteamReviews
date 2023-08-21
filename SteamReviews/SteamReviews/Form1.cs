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


			FreshTable();
			

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

		public string[] SpamNames = { "Bee Island", "Long Way", "Don Duality", "Sunkenland", "Absence of Light" };
		public string[] SpamLinks = { "https://store.steampowered.com/app/2345020/Bee_Island/", "https://store.steampowered.com/app/2455070/Long_Way/", "https://store.steampowered.com/app/2247570/Don_Duality/", "https://store.steampowered.com/app/2080690/Sunkenland/", "https://store.steampowered.com/app/2536710/Absence_of_Light/" };


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

		public void FreshTable()
		{
			string[] subdirectories = Directory.GetDirectories(@"Steam/");

			foreach (string Dir in subdirectories)
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


				string[] row = { mdir, count.ToString(), spammed.ToString(), closed.ToString() };
				dataGridView1.Rows.Add(row);
			}
		}

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
			{ "italian", "Ciao!\nPotrebbe interessarti.\nStiamo facendo un gioco simile! Abbiamo intenzione di finirlo entro la fine dell'estate!\nGuarda la pagina del nostro gioco.\nSe ti piace, aggiungi il gioco alla tua lista dei desideri, questo ci aiuterà nella promozione!\nGrazie e scusa per il disturbo!\n\nGioco:" },
			{ "polish", "Cześć!\nMoże Cię to zainteresuje.\nTworzymy podobną grę! Planujemy ją ukończyć do końca lata!\nSprawdź stronę naszej gry.\nJeśli Ci się spodoba, dodaj ją do listy życzeń, to pomoże w promocji!\nDziękujemy i przepraszamy za przeszkadzanie!\n\nGra:" },
			{ "schinese", "你好！\n也许你会感兴趣。\n我们正在制作一款类似的游戏！我们计划在夏季结束之前完成！\n请查看我们的游戏页面。\n如果你喜欢，请将游戏添加到愿望列表，这将有助于推广！\n谢谢，为打扰你感到抱歉！\n\n游戏：" },
			{ "tchinese", "你好！\n也許你會有興趣。\n我們正在製作一款類似的遊戲！我們計劃在夏季結束之前完成！\n請查看我們的遊戲頁面。\n如果你喜歡，請將遊戲添加到願望清單，這將有助於推廣！\n謝謝，為打擾你感到抱歉！\n\n遊戲：" },
			{ "japanese", "こんにちは！\n興味があるかもしれません。\n似たようなゲームを作っています！夏の終わりまでに完成する予定です！\n私たちのゲームページをご覧ください。\n気に入ったら、ゲームをウィッシュリストに追加してください、これはプロモーションに役立ちます！\nご迷惑をおかけして申し訳ありませんが、よろしくお願いします！\n\nゲーム：" },
			{ "koreana", "안녕하세요!\n관심 있으실지도 모릅니다.\n비슷한 게임을 만들고 있습니다! 여름이 끝나기 전에 완성할 계획입니다!\n저희 게임 페이지를 확인해보세요.\n마음에 드신다면 게임을 희망 목록에 추가해주시면 홍보에 도움이 됩니다!\n감사하고 불편을 끼쳐드려 죄송합니다!\n\n게임:" },
			{ "thai", "สวัสดี!\nอาจที่คุณจะสนใจ\nเรากำลังทำเกมที่คล้ายกัน! เราวางแผนที่จะเสร็จสิ้นภายในปลายฤดูร้อน!\nดูหน้าเกมของเรา\nถ้าชอบ ให้เพิ่มเกมลงในรายการที่ต้องการ นั่นจะช่วยส่งเสริม!\nขอบคุณและขออภัยหากขัดจังหวะ!\n\nเกม:" },
			{ "bulgarian", "Здравей!\nМоже да те заинтересува.\nПравим подобна игра! Планираме да я завършим до края на лятото!\nПровери страницата на нашата игра.\nАко ти хареса, добави играта в желаните, това ще помогне за нейното популяризиране!\nБлагодарим и се извиняваме за безпокойството!\n\nИгра:" },
			{ "czech", "Ahoj!\nMožná tě to zaujme.\nDěláme podobnou hru! Plánujeme ji dokončit ke konci léta!\nPodívej se na stránku naší hry.\nPokud se ti líbí, přidej si ji do seznamu přání, to nám pomůže s propagací!\nDěkujeme a omlouváme se za obtěžování!\n\nHra:" },
			{ "danish", "Hej!\nMåske kunne det interessere dig.\nVi laver et lignende spil! Vi planlægger at færdiggøre det inden sommerens slutning!\nKig på vores spilsiden.\nHvis du kan lide det, tilføj spillet til din ønskeliste, det vil hjælpe med markedsføringen!\nTak og undskyld for besværet!\n\nSpil:" },
			{ "german", "Hallo!\nVielleicht interessiert es dich.\nWir machen ein ähnliches Spiel! Wir planen, es bis zum Ende des Sommers abzuschließen!\nSchau dir die Seite unseres Spiels an.\nWenn es dir gefällt, füge das Spiel deiner Wunschliste hinzu, das hilft bei der Verbreitung!\nDanke und entschuldige die Störung!\n\nSpiel:" },
			{ "english", "Hello!\nYou might be interested.\nWe are making a similar game! We plan to finish it by the end of summer!\nCheck out the page of our game.\nIf you like it, add the game to your wishlist, it will help with promotion!\nThank you and sorry for the inconvenience!\n\nGame:" },
			{ "spanish", "¡Hola!\nQuizás te interese.\n¡Estamos haciendo un juego similar! ¡Planeamos terminarlo para finales del verano!\nEcha un vistazo a la página de nuestro juego.\nSi te gusta, añade el juego a tu lista de deseos, ¡esto ayudará en la promoción!\n¡Gracias y disculpa las molestias!\n\nJuego:" },
			{ "latam", "¡Hola!\nQuizá te interese.\n¡Estamos haciendo un juego similar! ¡Planeamos terminarlo para finales del verano!\nEcha un vistazo a la página de nuestro juego.\nSi te gusta, agrega el juego a tu lista de deseos, ¡esto ayudará en la promoción!\n¡Gracias y disculpa las molestias!\n\nJuego:" },
			{ "greek", "Γεια σου!\nΊσως σε ενδιαφέρει.\nΦτιάχνουμε ένα παρόμοιο παιχνίδι! Προγραμματίζουμε να το ολοκληρώσουμε μέχρι το τέλος του καλοκαιριού!\nΚοίτα τη σελίδα του παιχνιδιού μας.\nΑν σου αρέσει, πρόσθεσε το παιχνίδι στη λίστα επιθυμιών σου, αυτό θα βοηθήσει στην προώθηση!\nΕυχαριστούμε και συγνώμη για την ταλαιπωρία!\n\nΠαιχνίδι:" },
			{ "french", "Salut !\nCela pourrait t'intéresser.\nNous faisons un jeu similaire ! Nous prévoyons de le terminer d'ici la fin de l'été !\nJette un œil à la page de notre jeu.\nSi ça te plaît, ajoute le jeu à ta liste de souhaits, ça nous aidera pour la promotion !\nMerci et désolé pour le dérangement !\n\nJeu:" },
			{ "hungarian", "Szia!\nLehet, hogy érdekel.\nHasonló játékot készítünk! A terveink szerint nyár végére befejezzük!\nNézd meg a játékunk oldalát.\nHa tetszik, adj hozzá a játékot a kívánságlistádhoz, ez segíteni fog a népszerűsítésben!\nKöszönjük és elnézést a kellemetlenségért!\n\nJáték:" },
			{ "dutch", "Hallo!\nMisschien ben je geïnteresseerd.\nWe maken een vergelijkbaar spel! We zijn van plan het tegen het einde van de zomer af te ronden!\nBekijk de pagina van ons spel.\nAls je het leuk vindt, voeg het spel dan toe aan je verlanglijstje, dat helpt bij de promotie!\nBedankt en sorry voor het ongemak!\n\nSpel:" },
			{ "norwegian", "Hei!\nKanskje du er interessert.\nVi lager et lignende spill! Vi planlegger å fullføre det mot slutten av sommeren!\nSjekk ut siden til spillet vårt.\nHvis du liker det, legg til spillet på ønskelisten din, det vil hjelpe med markedsføringen!\nTakk og beklager for bryderiet!\n\nSpill:" },
			{ "portuguese", "Olá!\nVocê pode se interessar.\nEstamos fazendo um jogo semelhante! Planejamos terminá-lo até o final do verão!\nConfira a página do nosso jogo.\nSe você gostar, adicione o jogo à sua lista de desejos, isso ajudará na promoção!\nObrigado e desculpe pelo incômodo!\n\nJogo:" },
			{ "brazilian", "Olá!\nVocê pode se interessar.\nEstamos fazendo um jogo similar! Planejamos terminá-lo até o final do verão!\nConfira a página do nosso jogo.\nSe gostar, adicione o jogo à sua lista de desejos, isso ajudará na promoção!\nObrigado e desculpe pelo incômodo!\n\nJogo:" },
			{ "romanian", "Salut!\nPoate te interesează.\nFacem un joc similar! Planificăm să-l terminăm până la sfârșitul verii!\nVerifică pagina jocului nostru.\nDacă îți place, adaugă jocul în lista ta de dorințe, acest lucru va ajuta la promovare!\nMulțumim și ne cerem scuze pentru inconveniențe!\n\nJoc:" },
			{ "russian", "Привет!\nВозможно тебя заинтересует.\nМы делаем похожую игру! Планируем завершить её к концу лета!\nПосмотри страницу нашей игры.\nЕсли понравится - добавь игру в список желаемого, это поможет в продвижении!\nСпасибо и извини за беспокойство!\n\nИгра:" },
			{ "finnish", "Hei!\nSaattaisi kiinnostaa.\nTeemme samanlaista peliä! Suunnitelmissamme on saada se valmiiksi kesän loppuun mennessä!\nTutustu pelimme sivuun.\nJos tykkäät, lisää peli toivelistaasi, se auttaa markkinoinnissa!\nKiitos ja anteeksi häiriöstä!\n\nPeli:" },
			{ "swedish", "Hej!\nDu kanske är intresserad.\nVi gör en liknande spel! Vi planerar att slutföra det innan sommarens slut!\nKolla in sidan för vårt spel.\nOm du gillar det, lägg till spelet i din önskelista, det hjälper till med marknadsföringen!\nTack och förlåt för besväret!\n\nSpel:" },
			{ "turkish", "Merhaba!\nBelki ilgilenirsin.\nBenzer bir oyun yapıyoruz! Yaz sonuna kadar bitirmeyi planlıyoruz!\nOyunumuzun sayfasına göz at.\nBeğenirsen, oyunu dilek listene ekle, bu tanıtımda yardımcı olacaktır!\nTeşekkür ederiz ve rahatsızlık için özür dileriz!\n\nOyun:" },
			{ "vietnamese", "Xin chào!\nCó thể bạn quan tâm.\nChúng tôi đang làm một trò chơi tương tự! Kế hoạch hoàn thành trước cuối mùa hè!\nHãy xem trang của trò chơi chúng tôi.\nNếu bạn thích, thêm trò chơi vào danh sách mong muốn, điều đó sẽ giúp quảng bá!\nCảm ơn bạn và xin lỗi vì sự phiền toái!\n\nTrò chơi:" },
			{ "ukrainian", "Привіт!\nМожливо, тебе зацікавить.\nМи робимо подібну гру! Плануємо завершити її до кінця літа!\nПереглянь сторінку нашої гри.\nЯкщо сподобається - додай гру до списку бажаного, це допоможе в продвиженні!\nДякуємо і вибач за турботу!\n\nГра:" }
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
								text += SpamNames[SpamerNumber];
								text += "\n";
								text += SpamLinks[SpamerNumber];

								Random random = new Random();
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
								}

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

			FreshTable(); 

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
