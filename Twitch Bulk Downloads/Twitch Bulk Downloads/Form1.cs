using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitch_Bulk_Downloads
{
    public partial class Form1 : Form
    {
        string folderPath = "";
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderPath == "")
            {
                MessageBox.Show("Select Destination Folder!!!");
            }
            else
            {
                string links = richTextBox1.Text;
                if(links == "")
                {
                    MessageBox.Show("Enter Links");
                }
                else
                {
                    string[] links_divided = links.Split(",");
                    for (int i = 0; i < links_divided.Length; i++)
                    {
                        downloadfile(links_divided[i], folderPath, i + 1);
                    }
                    MessageBox.Show("Downloads Completed!!");
                }
            }
        }

        private void downloadfile(string Link, string SavePath, int index)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", SavePath);
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

            if(checkBox1.Checked)
                chromeOptions.AddArguments("--headless");

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            IWebDriver driver = new ChromeDriver(driverService, chromeOptions);
            driver.Navigate().GoToUrl("https://clipr.xyz/");
            IWebElement ele = driver.FindElement(By.Id("clip_url"));
            ele.SendKeys(Link);
            IWebElement ele1 = driver.FindElement(By.XPath("/html/body/div[2]/main/div[1]/div/div/div/div/form/button"));
            ele1.Click();
            

            bool FoundDownloadLink = true;
            IWebElement ele2 = null;
            while (FoundDownloadLink)
            {
                try
                {
                    ele2 = driver.FindElement(By.XPath("/html/body/div[2]/main/div[1]/div/div/div/div/div[2]/div[2]/div[1]/a[2]"));
                    FoundDownloadLink = false;
                }
                catch
                {
                    Thread.Sleep(25);
                }
            }
            string FileName = ele2.GetAttribute("href");
            string[] FileNames = FileName.Split("/");
            FileName = FileNames[FileNames.Length - 1];
            FileName = FileName.Replace("%7C", "_");
            ele2.Click();

            bool DownloadComplete = true;
            while (DownloadComplete)
            {
                if (File.Exists(SavePath + "\\" + FileName))
                    DownloadComplete = false;
                else
                    Thread.Sleep(25);
            }

            if(checkBox2.Checked)
                File.Move(SavePath + "\\" + FileName, SavePath + "\\" + index + ".mp4");

            driver.Quit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
            }

            label3.Text = folderPath;
        }
    }
}
