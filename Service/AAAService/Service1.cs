using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MyLib;
using Timer = System.Timers.Timer;

namespace AAAService
{
    public partial class AAAService : ServiceBase
    {
        public static readonly Mutex TextMutex = new Mutex(false, @"Global\TextMutex");
        public static readonly Mutex LinksMutex = new Mutex(false, @"Global\LinksMutex");
        public static readonly Mutex ImagesMutex = new Mutex(false, @"Global\ImagesMutex");
        public AAAService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer {Interval = 5000};
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            StartThreads();
        }

        protected override void OnStop()
        {
        }
        
        private static void PushTexts()
        {
            TextMutex.WaitOne();
            var texts = @"C:\Users\Александр\Desktop\НЕ МОЕ\SeleniumApplications\jsons\texts.json".DeserializeTo<List<Text>>();
            TextMutex.ReleaseMutex();
            var context = new Context();
            var dataTexts = context.Texts.ToList();
            foreach (var jsonEl in texts)
            {
                if (dataTexts.Where(dataEl => dataEl.FeedId == jsonEl.Id).ToList().Count < 1)
                    context.Texts.Add(new DbText(jsonEl.Id, jsonEl.ContentText));
            }
            context.SaveChanges();
        }
        private static void PushLinks()
        {
            LinksMutex.WaitOne();
            var links = @"C:\Users\Александр\Desktop\НЕ МОЕ\SeleniumApplications\jsons\links.json".DeserializeTo<List<Link>>();
            LinksMutex.ReleaseMutex();
            var context = new Context();
            var dataLinks = context.Links.ToList();
            foreach (var jsonEl in links)
            {
                if (dataLinks.Where(dataEl => dataEl.FeedId == jsonEl.Id).ToList().Count < 1)
                    context.Links.Add(new DbLink(jsonEl.Id, jsonEl.ContentLinks?.ToStr()));
            }
            context.SaveChanges();
        }

        private static void PushImages()
        {
            ImagesMutex.WaitOne();
            var images = @"C:\Users\Александр\Desktop\НЕ МОЕ\SeleniumApplications\jsons\images.json".DeserializeTo<List<Link>>();
            ImagesMutex.ReleaseMutex();
            var context = new Context();
            var dataImages = context.Images.ToList();
            foreach (var jsonEl in images)
            {
                if (dataImages.Where(dataEl => dataEl.FeedId == jsonEl.Id).ToList().Count < 1)
                    context.Images.Add(new DbImage(jsonEl.Id, jsonEl.ContentLinks?.ToStr()));
            }
            context.SaveChanges();
        }
        public static void StartThreads()
        {
            new List<Thread> { new Thread(PushTexts), new Thread(PushLinks), new Thread(PushImages) }.ForEach(t => t.Start());
        }
    }
}
