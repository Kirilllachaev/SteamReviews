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
using System.Threading;
using System.Collections.ObjectModel;

namespace SteamReviews
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			selectedLanguage = comboBox1.Text;
		}

		ChromeDriver driver = null;
		bool isScrolling = false;
		Thread ScrollingThread = null;
		Thread MultipleLangsThread = null;

		public int pageNumber = 0;

		public List<string> Alllinks = new List<string>();

		public string selectedLanguage = "all";


		public string gameName = "";
		public string gameLink = "";



		public string linksFile = "";

		public int allLinkCount = 0;







		private void button1_Click_1(object sender, EventArgs e)
		{
			gameName = textBox2.Text;
			gameLink = textBox1.Text;


			if (selectedLanguage == "all")
			{
				MultipleLangsThread = new Thread(() => MultipleLangs());
				MultipleLangsThread.Start();
			}
			else
			{
				driver = new ChromeDriver();
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




			}



		}

		public void MultipleLangs()
		{

			for (int i = 1; i < comboBox1.Items.Count; i++)
			{
				string CselectedLanguage = comboBox1.Items[i].ToString();

				driver = new ChromeDriver();
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

				

				driver.Quit();


			}

			Thread t = new Thread(() => MessageBox.Show("Сбор ссылок завершён. Было собрано " + allLinkCount.ToString() + " ссылок", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information));
			t.Start();

			Thread.Sleep(500);


		}






		private void button2_Click(object sender, EventArgs e)
		{
			isScrolling = true;

			ScrollingThread = new Thread(() => Scrolling(driver, true, selectedLanguage));
			ScrollingThread.Start();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			isScrolling = false;
			pageNumber = 0;
			allLinkCount = 0;
			Alllinks.Clear();

			if (driver != null)
				driver.Quit();
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
		}

		private void button6_Click(object sender, EventArgs e)
		{
			IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
			greenButton.Click();
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
				System.IO.File.WriteAllLines(@_gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
			}
			else
			{
				System.IO.Directory.CreateDirectory(gameName);
				System.IO.File.WriteAllLines(@_gameName + "/" + _gameName + "_" + _selectedLanguage + ".txt", Alllinks);
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
	}
}
