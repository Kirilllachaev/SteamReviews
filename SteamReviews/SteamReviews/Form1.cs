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

		Dictionary<string, string> messages = new Dictionary<string, string>
		{
			{ "italian", "Ciao!\nPotrebbe interessarti.\nStiamo facendo un gioco simile! Abbiamo intenzione di finirlo entro la fine dell'estate!\nDai un'occhiata alla pagina del nostro gioco Bee Island.\nSe ti piace, aggiungi il gioco alla tua lista dei desideri e unisciti al nostro server Discord. Questo ci aiuterà nella promozione!\nGrazie e scusa per il disturbo!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "polish", "Cześć!\nMoże Cię to zainteresuje.\nTworzymy podobną grę! Planujemy ją ukończyć do końca lata!\nZajrzyj na stronę naszej gry Bee Island.\nJeśli Ci się spodoba, dodaj ją do listy życzeń i dołącz do naszego serwera Discord. To pomoże w promocji!\nDziękujemy i przepraszamy za przeszkadzanie!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "schinese", "你好！\n也许你会感兴趣。\n我们正在制作一款类似的游戏！我们计划在夏季结束之前完成！\n请查看我们的游戏《蜜蜂岛》页面。\n如果你喜欢，请将游戏添加到愿望列表，并加入我们的 Discord 服务器，这将有助于推广！\n谢谢，为打扰你感到抱歉！\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "tchinese", "你好！\n也許你會有興趣。\n我們正在製作一款類似的遊戲！我們計劃在夏季結束之前完成！\n請查看我們的遊戲《蜜蜂島》頁面。\n如果你喜歡，請將遊戲添加到願望清單並加入我們的 Discord 伺服器，這將有助於推廣！\n謝謝，為打擾你感到抱歉！\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "japanese", "こんにちは！\n興味があるかもしれません。\n似たようなゲームを作っています！夏の終わりまでに完成する予定です！\n私たちのゲーム「Bee Island」のページをご覧ください。\n気に入ったら、ゲームをウィッシュリストに追加し、Discordサーバーに参加してください。これはプロモーションに役立ちます！\nご迷惑をおかけして申し訳ありませんが、よろしくお願いします！\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "koreana", "안녕하세요!\n관심 있으실지도 모릅니다.\n비슷한 게임을 만들고 있습니다! 여름이 끝나기 전에 완성할 계획입니다!\n저희 게임 Bee Island 페이지를 확인해보세요.\n마음에 드신다면 게임을 희망 목록에 추가하고 Discord 서버에 참여해주시면 홍보에 도움이 됩니다!\n감사하고 불편을 끼쳐드려 죄송합니다!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "thai", "สวัสดี!\nอาจที่คุณจะสนใจ\nเรากำลังทำเกมที่คล้ายกัน! เราวางแผนที่จะเสร็จสิ้นภายในปลายฤดูร้อน!\nเข้าไปดูหน้าเกมของเรา Bee Island\nถ้าชอบ ให้เพิ่มเกมลงในรายการที่ต้องการและเข้ามายังเซิร์ฟเวอร์ Discord ของเรา นั่นจะช่วยส่งเสริม!\nขอบคุณและขออภัยหากขัดจังหวะ!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "bulgarian", "Здравей!\nМоже да те заинтересува.\nПравим подобна игра! Планираме да я завършим до края на лятото!\nПровери страницата на нашата игра Bee Island.\nАко ти хареса, добави играта в желаните и влез в нашия Discord сървър, това ще помогне за нейното популяризиране!\nБлагодарим и се извиняваме за безпокойството!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "czech", "Ahoj!\nMožná tě to zaujme.\nDěláme podobnou hru! Plánujeme ji dokončit ke konci léta!\nPodívej se na stránku naší hry Bee Island.\nPokud se ti líbí, přidej si ji do seznamu přání a připoj se na náš Discord server, to nám pomůže s propagací!\nDěkujeme a omlouváme se za obtěžování!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "danish", "Hej!\nMåske kunne det interessere dig.\nVi laver et lignende spil! Vi planlægger at færdiggøre det inden sommerens slutning!\nKig på vores spilsiden Bee Island.\nHvis du kan lide det, tilføj spillet til din ønskeliste og kom ind på vores Discord-server, det vil hjælpe med markedsføringen!\nTak og undskyld for besværet!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "german", "Hallo!\nVielleicht interessiert es dich.\nWir machen ein ähnliches Spiel! Wir planen, es bis zum Ende des Sommers abzuschließen!\nSchau dir die Seite unseres Spiels Bee Island an.\nWenn es dir gefällt, füge das Spiel deiner Wunschliste hinzu und komm auf unseren Discord-Server, das hilft bei der Verbreitung!\nDanke und entschuldige die Störung!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "english", "Hello!\nYou might be interested.\nWe are making a similar game! We plan to finish it by the end of summer!\nCheck out the page of our game Bee Island.\nIf you like it, add the game to your wishlist and join our Discord server, it will help with promotion!\nThank you and sorry for the inconvenience!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "spanish", "¡Hola!\nQuizás te interese.\n¡Estamos haciendo un juego similar! ¡Planeamos terminarlo para finales del verano!\nEcha un vistazo a la página de nuestro juego Bee Island.\nSi te gusta, añade el juego a tu lista de deseos y únete a nuestro servidor de Discord, ¡esto ayudará en la promoción!\n¡Gracias y disculpa las molestias!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "latam", "¡Hola!\nQuizá te interese.\n¡Estamos haciendo un juego similar! ¡Planeamos terminarlo para finales del verano!\nEcha un vistazo a la página de nuestro juego Bee Island.\nSi te gusta, agrega el juego a tu lista de deseos y únete a nuestro servidor de Discord, ¡esto ayudará en la promoción!\n¡Gracias y disculpa las molestias!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "greek", "Γεια σου!\nΊσως σε ενδιαφέρει.\nΦτιάχνουμε ένα παρόμοιο παιχνίδι! Προγραμματίζουμε να το ολοκληρώσουμε μέχρι το τέλος του καλοκαιριού!\nΚοίτα τη σελίδα του παιχνιδιού μας Bee Island.\nΑν σου αρέσει, πρόσθεσε το παιχνίδι στη λίστα επιθυμιών σου και έλα στον διακομιστή Discord μας, αυτό θα βοηθήσει στην προώθηση!\nΕυχαριστούμε και συγνώμη για την ταλαιπωρία!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "french", "Salut !\nCela pourrait t'intéresser.\nNous faisons un jeu similaire ! Nous prévoyons de le terminer d'ici la fin de l'été !\nJette un œil à la page de notre jeu Bee Island.\nSi ça te plaît, ajoute le jeu à ta liste de souhaits et rejoins notre serveur Discord, ça nous aidera pour la promotion !\nMerci et désolé pour le dérangement !\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "hungarian", "Szia!\nLehet, hogy érdekel.\nHasonló játékot készítünk! A terveink szerint nyár végére befejezzük!\nNézd meg a Bee Island játékunk oldalát.\nHa tetszik, add hozzá a játékot a kívánságlistádhoz, és csatlakozz a Discord szerverünkhöz, ez segíteni fog a népszerűsítésben!\nKöszönjük és elnézést a kellemetlenségért!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "dutch", "Hallo!\nMisschien ben je geïnteresseerd.\nWe maken een vergelijkbaar spel! We zijn van plan het tegen het einde van de zomer af te ronden!\nBekijk de pagina van ons spel Bee Island.\nAls je het leuk vindt, voeg het spel dan toe aan je verlanglijstje en doe mee aan onze Discord-server, dit helpt bij de promotie!\nBedankt en sorry voor het ongemak!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "norwegian", "Hei!\nKanskje du er interessert.\nVi lager et lignende spill! Vi planlegger å fullføre det mot slutten av sommeren!\nSjekk ut siden til spillet vårt Bee Island.\nHvis du liker det, legg til spillet på ønskelisten din og bli med i vår Discord-server, det vil hjelpe med markedsføringen!\nTakk og beklager for bryderiet!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "portuguese", "Olá!\nVocê pode se interessar.\nEstamos fazendo um jogo semelhante! Planejamos terminá-lo até o final do verão!\nConfira a página do nosso jogo Bee Island.\nSe você gostar, adicione o jogo à sua lista de desejos e entre no nosso servidor do Discord, isso ajudará na promoção!\nObrigado e desculpe pelo incômodo!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "brazilian", "Olá!\nVocê pode se interessar.\nEstamos fazendo um jogo similar! Planejamos terminá-lo até o final do verão!\nConfira a página do nosso jogo Bee Island.\nSe gostar, adicione o jogo à sua lista de desejos e entre em nosso servidor do Discord, isso ajudará na promoção!\nObrigado e desculpe pelo incômodo!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "romanian", "Salut!\nPoate te interesează.\nFacem un joc similar! Planificăm să-l terminăm până la sfârșitul verii!\nVerifică pagina jocului nostru Bee Island.\nDacă îți place, adaugă jocul în lista ta de dorințe și alătură-te serverului nostru de Discord, acest lucru va ajuta la promovare!\nMulțumim și ne cerem scuze pentru inconveniențe!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "russian", "Привет!\nВозможно тебя заинтересует.\nМы делаем похожую игру! Планируем завершить её к концу лета!\nПосмотри страницу нашей игры Bee Island.\nЕсли понравится - добавь игру в список желаемого и заходи в наш Discord сервер, это поможет в продвижении!\nСпасибо и извини за беспокойство!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "finnish", "Hei!\nSaattaisi kiinnostaa.\nTeemme samanlaista peliä! Suunnitelmissamme on saada se valmiiksi kesän loppuun mennessä!\nTutustu pelimme Bee Island sivuun.\nJos tykkäät, lisää peli toivelistaasi ja liity Discord-palvelimeemme, se auttaa markkinoinnissa!\nKiitos ja anteeksi häiriöstä!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "swedish", "Hej!\nDu kanske är intresserad.\nVi gör en liknande spel! Vi planerar att slutföra det innan sommarens slut!\nKolla in sidan för vårt spel Bee Island.\nOm du gillar det, lägg till spelet i din önskelista och gå med i vår Discord-server, det hjälper till med marknadsföringen!\nTack och förlåt för besväret!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "turkish", "Merhaba!\nBelki ilgilenirsin.\nBenzer bir oyun yapıyoruz! Yaz sonuna kadar bitirmeyi planlıyoruz!\nBee Island oyunumuzun sayfasına göz at.\nBeğenirsen, oyunu dilek listene ekle ve Discord sunucumuza katıl, bu tanıtımda yardımcı olacaktır!\nTeşekkür ederiz ve rahatsızlık için özür dileriz!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "vietnamese", "Xin chào!\nCó thể bạn quan tâm.\nChúng tôi đang làm một trò chơi tương tự! Kế hoạch hoàn thành trước cuối mùa hè!\nHãy xem trang của trò chơi Bee Island của chúng tôi.\nNếu bạn thích, thêm trò chơi vào danh sách mong muốn và tham gia Discord server của chúng tôi, điều đó sẽ giúp quảng bá!\nCảm ơn bạn và xin lỗi vì sự phiền toái!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" },
			{ "ukrainian", "Привіт!\nМожливо, тебе зацікавить.\nМи робимо подібну гру! Плануємо завершити її до кінця літа!\nПереглянь сторінку нашої гри Bee Island.\nЯкщо сподобається - додай гру до списку бажаного та заходь на наш Discord сервер, це допоможе в продвиженні!\nДякуємо і вибач за турботу!\n\nhttps://store.steampowered.com/app/2345020/Bee_Island/" }
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
								textarea.SendKeys(text);
								Thread.Sleep(1000);
								IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
								greenButton.Click();

								RL.Links[RL.Links.IndexOf(L)] += "?donsk";
								Thread.Sleep(500);


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
