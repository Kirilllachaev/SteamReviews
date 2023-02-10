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

		public int pageNumber = 0;

		public List<string> Alllinks = new List<string>();

		public string selectedLanguage = "all";

		private void button1_Click_1(object sender, EventArgs e)
		{
			driver = new ChromeDriver();
			
			driver.Navigate().GoToUrl("https://steamcommunity.com/app/701160/reviews" + "/?p=1&browsefilter=all&filterLanguage=" + selectedLanguage);
		}


		private void button2_Click(object sender, EventArgs e)
		{
			isScrolling = true;
			ScrollingThread = new Thread(new ThreadStart(Scrolling));
			ScrollingThread.Start();


		}

		private void button3_Click(object sender, EventArgs e)
		{
			//	IWebElement greenButton = driver.FindElement(By.ClassName("btn_green_white_innerfade"));
			//	greenButton.Click();

			isScrolling = false;
			MakeOutput();




		}

	

		private void button4_Click(object sender, EventArgs e)
		{
			driver.Quit();
			pageNumber = 0;
			Alllinks.Clear();
		}






		public void Scrolling()
		{
			while (isScrolling)
			{
				

				try
				{
					((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

					pageNumber += 1;
					IWebElement page = driver.FindElement(By.Id("page" + pageNumber.ToString()));

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
					isScrolling = false;
					MakeOutput();
				}
				
			}
		}


		public void MakeOutput()
		{
			System.IO.File.WriteAllLines("kingdom_links_" + selectedLanguage + ".txt", Alllinks);
			
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedLanguage = comboBox1.Text;
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}
}
