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
    using Castle.Components.DictionaryAdapter.Xml;

    [Category("RunOnlyThis")]
    public class GhostStoryGenerator
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

        public void GeneratorTest()
        {
            string publicIp = "127.0.0.1";

            //string claude_url = "https://app.slack.com/client/xxxxxxxxxxxxxxxxxx"; beta
            string claude_url = "https://app.slack.com/client/xxxxxxxxxxxxxxxxxxxxxxx";
            string claude_login_name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            //string claude_spacename = "xxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            string password = "xxxxxxxxxxxxxxxxxxxxxxxx";
            string claude_spacename = "xxxxxxxxxxxxxxxxxxxxx";
            string narakeet_url = "https://www.narakeet.com/app/text-to-audio/?projectId=6e01ee9c-86c5-48df-9209-d8316e06ef08";

            GhostStoryGeneratorSteps(publicIp, claude_url, claude_spacename, claude_login_name, password, narakeet_url);

        }

        private void GhostStoryGeneratorSteps(string publicIp, 
                                              string claude_url,
                                              string claude_spacename,
                                              string claude_login_name,
                                              string password,
                                              string narakeet_url)
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


            //===================step1: generate the ghost story
            string story_content_zn = Step1_SlackGenerateTheStory(wdriver, claude_url, claude_spacename, claude_login_name, password);
            string story_content_english = Step1_SlackGenerateTheStoryEnglish(wdriver, claude_url, claude_spacename, claude_login_name, password);
            
            //===================step2: generate the narakeet video
            string audiomp3_zn = Step2_NarakeetGenerateTheAudio(wdriver, narakeet_url, story_content_zn);
            Step2_NarakeetGenerateTheAudioEnglish(wdriver, narakeet_url, story_content_english);
            wdriver.Quit();


        }

        public string Step1_SlackGenerateTheStory(RemoteWebDriver wdriver,string claude_url, string claude_spacename, string claude_login_name, string password)
        {
            //===== login slack =====
            Console.WriteLine("step1: generate the ghost story: claude_url=" + claude_url); ;
            wdriver.Navigate().GoToUrl(claude_url);
            wdriver.Manage().Window.Maximize();

            wdriver.FindElement(By.CssSelector("[data-qa='signin_domain_input']")).SendKeys(claude_spacename);
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[data-qa='submit_team_domain_button'")).Click();
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector(".c-google_login__label")).Click();
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[name='identifier']")).SendKeys(claude_login_name);
            wdriver.FindElement(By.CssSelector("[data-primary-action-label='Next'] [id='identifierNext']")).Click();
            Thread.Sleep(10000);
            wdriver.FindElement(By.CssSelector("[name='Passwd']")).SendKeys(password);
            wdriver.FindElement(By.CssSelector("[id='passwordNext']")).Click();
            //wdriver.FindElement(By.CssSelector("[data-qa='channel_ctx_menu_star']")).Click();
            Thread.Sleep(10000);
            wdriver.FindElement(By.CssSelector("div.ql-editor.ql-blank")).SendKeys("@Claude 请写一个鬼故事, 150字，开头这样写：今天是 "+ DateTime.Now.ToString("yyyy 年 MM 月 dd 日") +", 这是今天的鬼故事，请您听听。");
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[data-qa='texty_send_button']")).SendKeys(Keys.Enter);
            Thread.Sleep(15000);
            //wdriver.FindElement(By.CssSelector(".c-link--button.c-message__reply_count:nth-last-of-type(1)")).Click(); 
            //.c-virtual_list__item.c-virtual_list__item--initial-activeitem = thread
            wdriver.FindElement(By.CssSelector("[aria-posinset='1']")).Click();
            //wdriver.FindElement(By.CssSelector("'.c-texty_input_unstyled__container.c-texty_input_unstyled__container--size_medium.c-texty_input_unstyled__container--single_line.c-texty_input_unstyled__container--use_focus_ring.c-texty_input_unstyled__container--no_actions'")).SendKeys("鬼故事");
            Thread.Sleep(15000);
            //wdriver.FindElement(By.CssSelector("[data-qa='texty_send_button']")).SendKeys(Keys.Enter);
            string story_content = wdriver.FindElement(By.CssSelector(".p-client_container")).Text;
            //Console.WriteLine("story_content = " + story_content + "=============\n");
            story_content = story_content.Substring(story_content.IndexOf("APP  < 1 minute ago"), story_content.IndexOf("(edited)"));
            story_content = story_content.Replace("APP  < 1 minute ago", "");
            story_content = story_content.Replace(story_content.Substring(story_content.IndexOf("(edited)")), "");
            Console.WriteLine("index=" + story_content.IndexOf("(edited)") + "\nstory_content = " + story_content + "=============\n");
            //story_content = story_content.Replace("(edited)", "").Replace("Claude", "").Replace("Please not", "");
            //Console.WriteLine("story_content = " + story_content);
            return story_content;

        }

        public string Step1_SlackGenerateTheStoryEnglish(RemoteWebDriver wdriver, string claude_url, string claude_spacename, string claude_login_name, string password)
        {
            //===== login slack =====
            Console.WriteLine("step1: generate the ghost story: claude_url=" + claude_url); ;
            wdriver.Navigate().GoToUrl(claude_url);
            Thread.Sleep(10000);
            wdriver.FindElement(By.CssSelector("div.ql-editor.ql-blank")).SendKeys("@Claude Please write a ghost story, 120 words，start with：Today is " + DateTime.Now.ToString("yyyy-MM-dd") + ", this is today's ghost story, let's start.");
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("[data-qa='texty_send_button']")).SendKeys(Keys.Enter);
            Thread.Sleep(10000);
            //wdriver.FindElement(By.CssSelector(".c-link--button.c-message__reply_count:nth-last-of-type(1)")).Click(); 
            //.c-virtual_list__item.c-virtual_list__item--initial-activeitem = thread
            wdriver.FindElement(By.CssSelector("[aria-posinset='1']")).Click();
            //wdriver.FindElement(By.CssSelector("'.c-texty_input_unstyled__container.c-texty_input_unstyled__container--size_medium.c-texty_input_unstyled__container--single_line.c-texty_input_unstyled__container--use_focus_ring.c-texty_input_unstyled__container--no_actions'")).SendKeys("鬼故事");
            Thread.Sleep(15000);
            //wdriver.FindElement(By.CssSelector("[data-qa='texty_send_button']")).SendKeys(Keys.Enter);
            string story_content = wdriver.FindElement(By.CssSelector(".p-client_container")).Text;
            //Console.WriteLine("story_content = " + story_content + "=============\n");

            story_content = story_content.Substring(story_content.IndexOf("APP  < 1 minute ago"), story_content.IndexOf("(edited)"));
            Console.WriteLine("index=" + story_content.IndexOf("(edited)") + "\nstory_content = " + story_content + "=============\n");
            story_content = story_content.Replace("APP  < 1 minute ago", "");
            story_content = story_content.Replace(story_content.Substring(story_content.IndexOf("(edited)")), "");

            string replace_str = "";

            if (story_content.Contains("ghost story:")){
                replace_str = "ghost story:";

            }
            if (story_content.Contains("ghost story starting with the provided sentence:"))
            {
                replace_str = "ghost story starting with the provided sentence:";

            }

            if (story_content.Contains("ghost story starting with the given sentence:"))
            {
                replace_str = "ghost story starting with the given sentence:";

            }


            Console.WriteLine("start strring = " + story_content.Substring(0, story_content.IndexOf(replace_str))+ replace_str);

            story_content = story_content.Replace(story_content.Substring(0, story_content.IndexOf(replace_str))+ replace_str, "");
            Console.WriteLine("story_content = " + story_content);
            return story_content;

        }

        public string Step2_NarakeetGenerateTheAudio(RemoteWebDriver wdriver, string narakeet_url, string story_content_zn)
        {
            //============Send the story to text to Audio
            Console.WriteLine("step2: send the story to text to Audio");
            wdriver.Navigate().GoToUrl(narakeet_url);
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("select[id='cfgVideoLanguage'")).FindElement(By.CssSelector("[value='cmn-CN']")).Click();
            Thread.Sleep(5000);
            wdriver.FindElement(By.CssSelector("#cfgVideoVoice")).FindElement(By.CssSelector("[value='hetang']")).Click();
            Thread.Sleep(1000);
            wdriver.FindElement(By.CssSelector(".control-block button.button.round")).Click();
            Thread.Sleep(1000);
            wdriver.FindElement(By.CssSelector("#cfgAudioFormat")).FindElement(By.CssSelector("[value='mp3']")).Click();
            Thread.Sleep(1000);

            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).SendKeys(story_content_zn);
            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).SendKeys(Keys.Enter);
            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).Click();
            Thread.Sleep(10000);
            wdriver.FindElement(By.CssSelector("#workflowUploadForm button.button.primary")).Click();
            // wdriver.ExecuteJavaScript("document.getElementsByClassName('button.button.primary').click()");

            Thread.Sleep(15000);
            wdriver.FindElement(By.CssSelector(".actions.spaced.space-above .button.primary")).Click();
            Thread.Sleep(15000);
            string audio_mp3 = "";
            return audio_mp3;

        }
        public string Step2_NarakeetGenerateTheAudioEnglish(RemoteWebDriver wdriver, string narakeet_url, string story_content_en)
        {
            //============Send the story to text to Audio
            Console.WriteLine("step2: send the story to text to Audio");
            wdriver.Navigate().GoToUrl(narakeet_url);
            Thread.Sleep(5000);

            wdriver.FindElement(By.CssSelector(".control-block button.button.round")).Click();
            Thread.Sleep(1000);
            wdriver.FindElement(By.CssSelector("#cfgAudioFormat")).FindElement(By.CssSelector("[value='mp3']")).Click();
            Thread.Sleep(1000);

            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).SendKeys(story_content_en);
            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).SendKeys(Keys.Enter);
            wdriver.FindElement(By.CssSelector("#unparsedScriptEditor")).Click();
            Thread.Sleep(10000);
            wdriver.FindElement(By.CssSelector("#workflowUploadForm button.button.primary")).Click();
            // wdriver.ExecuteJavaScript("document.getElementsByClassName('button.button.primary').click()");

            Thread.Sleep(15000);
            wdriver.FindElement(By.CssSelector(".actions.spaced.space-above .button.primary")).Click();
            Thread.Sleep(15000);
            string audio_mp3 = "";
            return audio_mp3;

        }


      
        
       

    }
}
