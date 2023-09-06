namespace Tests
{

    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.UI;
    using Renci.SshNet;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using OpenQA.Selenium.Support;
    using OpenQA.Selenium.Support.Extensions;
    using System.Text;

    [Category("RunOnlyThat")]
    public class GhostStoryPoster
    {


        string env = System.Environment.MachineName;
        string currentEnv = "local"; //local, jenkins, teamcity
        string driverType = "remote"; //local, remote selenium driver
        string currentDir = Directory.GetCurrentDirectory();

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("###################Setup start");
            Console.WriteLine("current running in which environment = " + env);
            if (true)  
            {
                currentEnv = "local";
                if (currentDir.Contains("jenkins"))
                {
                    currentEnv = "jenkins";
                }
                if (currentDir.Contains("builds"))
                {
                    currentEnv = "teamcity";

                }

            }

            Console.WriteLine("currentEnv = " + currentEnv);
        }

        [Test]

        public void PosterTest()
        {
            string publicIp = "127.0.0.1";

            string yt_url = "https://studio.youtube.com/channel/xxxxxxxxxxxxxxxxxxxxxx";
            string yt_login_name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            string password = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
           
            string mp4_filename_en = @"C:\ghost_story\" + TestContext.Parameters.Get("mp4_filename_en", @"GhostStory_en.mp4").ToString();
            Console.WriteLine("mp4_filename en= " + mp4_filename_en);

            string mp4_filename_zn = @"C:\ghost_story\" + TestContext.Parameters.Get("mp4_filename_zn", @"GhostStory_zn.mp4").ToString();
            Console.WriteLine("mp4_filename = zn" + mp4_filename_zn);


            GhostStoryPosterSteps(publicIp, yt_url, yt_login_name, password, mp4_filename_en, mp4_filename_zn);

        }

        private void GhostStoryPosterSteps(string publicIp, 
                                      string yt_url,
                                      string yt_login_name,
                                      string password,
                                      string mp4_filename_en,
                                      string mp4_filename_zn
                                             )
        {
            Console.WriteLine("###################Test1=======");
            System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", @"c:\chromedriver.exe");

           

            //using (IWebDriver wdriver = new FirefoxDriver())
            RemoteWebDriver wdriver;
            ChromeOptions Options = new ChromeOptions();
            Options.PlatformName = "windows";
            Options.AddAdditionalCapability("platform", "WIN11", true);
            Options.AddUserProfilePreference("download.default_directory", "c:\\Users\\mxie\\Downloads");
            Options.AddUserProfilePreference("download.prompt_for_download", false);
            Options.AddUserProfilePreference("safebrowsing.enabled", true);
            Options.AddUserProfilePreference("download.directory_upgrade", true);
            Options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            Options.AddAdditionalCapability("version", "latest", true);
            wdriver = new RemoteWebDriver(new Uri("http://" + publicIp + ":4444/wd/hub/"), Options.ToCapabilities(), TimeSpan.FromSeconds(600));// NOTE: connection timeout of 600 seconds or more required for time to launch grid nodes if non are available.


            //===================step1: post to yt
            Step1_PostToYT(wdriver, yt_url, yt_login_name, password, mp4_filename_en, mp4_filename_zn);

           
            wdriver.Quit();


        }

        public void Step1_PostToYT(RemoteWebDriver wdriver,string yt_url, string yt_login_name, string password, string mp4_filename_en, string mp4_filename_zn)
        {
            string title_en = "";
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new StreamReader(@"C:\ghost_story\title_en.txt"))
                {
                    // Read the stream as a string, and write the string to the console.
                    title_en = sr.ReadToEnd();
                    Console.WriteLine("title txt content" + sr.ReadToEnd());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("title_en=" + title_en);

            string title_zn = "";
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new StreamReader(@"C:\ghost_story\title_zn.txt"))
                {
                    // Read the stream as a string, and write the string to the console.
                    title_zn = sr.ReadToEnd();
                    Console.WriteLine("title txt zn content" + sr.ReadToEnd());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("title_zn=" + title_zn);
            //===== login slack =====
            Console.WriteLine("step: post to yt cyt_url=" + yt_url); ;
             wdriver.Navigate().GoToUrl(yt_url);
             wdriver.Manage().Window.Maximize();

             Thread.Sleep(5000);
             wdriver.FindElement(By.CssSelector("[name='identifier']")).SendKeys(yt_login_name);
             wdriver.FindElement(By.CssSelector("[data-primary-action-label='Next'] [id='identifierNext']")).Click();
             Thread.Sleep(2000);
             wdriver.FindElement(By.CssSelector("[name='Passwd']")).SendKeys(password);
             wdriver.FindElement(By.CssSelector("[id='passwordNext']")).Click();
             // upload
             Thread.Sleep(5000);
             wdriver.FindElement(By.CssSelector("[id='create-icon']")).Click();
             wdriver.FindElement(By.CssSelector(".style-scope.ytcp-text-menu #text-item-0")).Click();
             Thread.Sleep(5000);
             //wdriver.FindElement(By.CssSelector("[id='select-files-button']")).Click();
             Thread.Sleep(3000);
             wdriver.FindElement(By.CssSelector("[id='select-files-button']")).SendKeys(mp4_filename_zn);
             wdriver.FindElement(By.CssSelector("[name='Filedata']")).SendKeys(mp4_filename_zn);
             //wdriver.FindElement(By.CssSelector(".style-scope.ytcp-uploads-file-picker-animation']")).SendKeys(@"C:\ghost_story\final.mp4");
             Thread.Sleep(5000);
             wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
             Thread.Sleep(3000);
             //step 1: details
             wdriver.FindElement(By.CssSelector("[id='child-input'] [id='container-content'] [id='textbox']")).SendKeys(" "+ title_zn + "故事");
             wdriver.FindElement(By.CssSelector("[name='VIDEO_MADE_FOR_KIDS_NOT_MFK'] ")).Click();
             wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
             Thread.Sleep(3000);
             //step 2: video elements
             wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
             Thread.Sleep(5000);
             //step 3: checks
             wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
             Thread.Sleep(5000);
             //step 4: visibility
             wdriver.FindElement(By.CssSelector("[name='PUBLIC'] [id='radioContainer']")).Click();
             wdriver.FindElement(By.CssSelector("[id='done-button']")).Click();
             Thread.Sleep(30000);
             wdriver.FindElement(By.CssSelector("[id='close-button'] div.label.style-scope.ytcp-button")).Click();

            //===== upload to yt en=====
            // upload
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[id='create-icon']")).Click();
            wdriver.FindElement(By.CssSelector(".style-scope.ytcp-text-menu #text-item-0")).Click();
            Thread.Sleep(5000);
            //wdriver.FindElement(By.CssSelector("[id='select-files-button']")).Click();
            Thread.Sleep(3000);
            wdriver.FindElement(By.CssSelector("[id='select-files-button']")).SendKeys(mp4_filename_en);
            wdriver.FindElement(By.CssSelector("[name='Filedata']")).SendKeys(mp4_filename_en);
            //wdriver.FindElement(By.CssSelector(".style-scope.ytcp-uploads-file-picker-animation']")).SendKeys(@"C:\ghost_story\final.mp4");
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
            Thread.Sleep(3000);
            //step 1: details
            wdriver.FindElement(By.CssSelector("[id='child-input'] [id='container-content'] [id='textbox']")).SendKeys(" " + title_en + "host story");
            wdriver.FindElement(By.CssSelector("[name='VIDEO_MADE_FOR_KIDS_NOT_MFK'] ")).Click();
            wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
            Thread.Sleep(3000);
            //step 2: video elements
            wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
            Thread.Sleep(5000);
            //step 3: checks
            wdriver.FindElement(By.CssSelector("[id='next-button']")).Click();
            Thread.Sleep(5000);
            //step 4: visibility
            wdriver.FindElement(By.CssSelector("[name='PUBLIC'] [id='radioContainer']")).Click();
            wdriver.FindElement(By.CssSelector("[id='done-button']")).Click();
            Thread.Sleep(30000);
            wdriver.FindElement(By.CssSelector("[id='close-button'] div.label.style-scope.ytcp-button")).Click();


        }

        

    }
}
