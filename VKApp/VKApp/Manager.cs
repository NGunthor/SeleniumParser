using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;
using MyLib;
using OpenQA.Selenium;

namespace VKApp
{
    public class Manager
    {
        public static readonly object TextsLocker = new object();
        public static readonly object LinksLocker = new object();
        public static readonly object ImagesLocker = new object();
        public static readonly Mutex TextMutex = new Mutex(false, @"Global\TextMutex");
        public static readonly Mutex LinksMutex = new Mutex(false, @"Global\LinksMutex");
        public static readonly Mutex ImagesMutex = new Mutex(false, @"Global\ImagesMutex");
        private List<Text> Texts { get; set; }
        private List<Link> Links { get; set; }
        private List<Link> Images { get; set; }
        private List<IWebElement> Wall { get; set; }
        private IWebDriver Driver { get; set; }

        public Manager(IWebDriver driver, string url)
        {
            Driver = driver;
            Driver.Navigate().GoToUrl(url);
            Driver.FindElement(By.Id("index_email")).SendKeys("79152373374");
            Driver.FindElement(By.Id("index_pass")).SendKeys("53aruheh" + Keys.Enter);

            Thread.Sleep(10000);

            MakeWall();
        }

        public void FindTexts()
        {
            Monitor.Enter(TextsLocker);
            Reader.NeedToReadTexts = true;
            Console.WriteLine("inFindTexts");
            Texts = new List<Text>();
            foreach (var feedRow in Wall)
                AddToTexts(feedRow)?.Invoke();
            TextMutex.WaitOne();
            WriteInFile(Texts, "texts.json");
            TextMutex.ReleaseMutex();
            Console.WriteLine("outFindTexts");
            Monitor.Exit(TextsLocker);
        }

        public void FindLinks()
        {
            Monitor.Enter(LinksLocker);
            Reader.NeedToReadLinks = true;
            Console.WriteLine("inFindLinks");
            Links = new List<Link>();
            foreach (var feedRow in Wall)
                AddToLinks(feedRow).Invoke();
            LinksMutex.WaitOne();
            WriteInFile(Links, "links.json");
            LinksMutex.ReleaseMutex();
            Console.WriteLine("outFindLinks");
            Monitor.Exit(LinksLocker);
        }

        public void FindImages()
        {
            Monitor.Enter(ImagesLocker);
            Reader.NeedToReadImages = true;
            Console.WriteLine("inFindImages");
            Images = new List<Link>();
            foreach (var feedRow in Wall)
                AddToImages(feedRow).Invoke();
            ImagesMutex.WaitOne();
            WriteInFile(Images, "images.json");
            ImagesMutex.ReleaseMutex();
            Console.WriteLine("outFindImages");
            Monitor.Exit(ImagesLocker);
        }

        private string GetId(IWebElement feedRow) =>
            feedRow.FindElement(By.XPath("./div")).GetAttribute("data-post-id");

        private static IWebElement GetTextFieldElement(IWebElement feedRow) =>
            feedRow.FindElement(By.ClassName("wall_text")).FindElement(By.XPath("./div/div"));

        [CanBeNull]
        private Action AddToTexts(IWebElement feedRow)
        {
            var textField = GetTextFieldElement(feedRow);
            textField =
                textField.GetAttribute("class") == "published_comment"
                    ? textField.FindElement(By.XPath("./div"))
                    : textField;
            if (textField.GetAttribute("class") != "wall_post_text" &&
                textField.GetAttribute("class") != "wall_post_text zoom_text")
                return () => Texts.Add(new Text(GetId(feedRow), null));
            if (textField.Text != string.Empty)
                return () => Texts.Add(new Text(GetId(feedRow), textField.GetAttribute("textContent")));
            return null;
        }

        private Action AddToLinks(IWebElement feedRow)
        {
            var textField = GetTextFieldElement(feedRow);
            var aElements = textField.FindElements(By.TagName("a"))
                .Where(element => element.GetAttribute("href") != null).ToList();
            if (textField.GetAttribute("class") != "wall_post_text" || aElements.Count == 0)
                return () => Links.Add(new Link(GetId(feedRow), null));
            var contentLinks = new List<string>();
            foreach (var element in aElements)
            {
                if (element.GetAttribute("href") != (string.Empty))
                    contentLinks.Add(element.GetAttribute("href"));
            }

            return () => Links.Add(new Link(GetId(feedRow), contentLinks));
        }

        private Action AddToImages(IWebElement feedRow)
        {
            var imageField = GetImageFieldElement(feedRow);
            if (imageField == null || imageField.GetAttribute("class") != "page_post_sized_thumbs  clear_fix")
                return () => Images.Add(new Link(GetId(feedRow), null));
            return () => Images.Add(new Link(GetId(feedRow),
                imageField.FindElements(By.TagName("a")).Select(GetImageUrl).ToList()));
        }

        [CanBeNull]
        private IWebElement GetImageFieldElement(IWebElement feedRow) =>
            feedRow.FindElement(By.ClassName("wall_text")).FindElements(By.XPath("./div/div[2]")).SingleOrDefault();

        private string GetImageUrl(IWebElement aElement) =>
            Regex.Match(aElement.GetAttribute("style"), @"https(.*?)jpg").ToString();


        private static void WriteInFile<T>(List<T> collection, string filename) where T : Post
        {
            var jsonFormatter = new DataContractJsonSerializer(collection.GetType());
            using (var file = new FileStream($@"C:\Users\Александр\Desktop\НЕ МОЕ\SeleniumApplications\jsons\{filename}", FileMode.Create))
                jsonFormatter.WriteObject(file, collection);
        }


        private void MakeWall()
        {
            IList<IWebElement> wall = Driver.FindElements(By.ClassName("feed_row"));
            Wall = new List<IWebElement>();
            foreach (var feedRow in wall)
            {
                if ((GetId(feedRow) == null) ||
                    (feedRow.FindElement(By.XPath("./div")).GetAttribute("data-ad") != null))
                    continue;
                Wall.Add(feedRow);
            }
        }
    }
}