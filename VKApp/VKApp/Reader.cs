using System;
using System.IO;
using System.Threading;

namespace VKApp
{
    public static class Reader
    {
        public static bool NeedToReadTexts { get; set; }
        public static bool NeedToReadLinks { get; set; }
        public static bool NeedToReadImages { get; set; }
        
        public static void RunReading()
        {
            Console.WriteLine("StartReading");
            while (NeedToReadTexts || NeedToReadLinks || NeedToReadImages)
            {
                if (NeedToReadTexts && Monitor.TryEnter(Manager.TextsLocker))
                {
                    Console.WriteLine("StReadTexts");
                    Manager.TextMutex.WaitOne();
                    ReadFile("texts.json");
                    Manager.TextMutex.ReleaseMutex();
                    NeedToReadTexts = false;
                    Monitor.Exit(Manager.TextsLocker);
                    Console.WriteLine("EndReadTexts");
                }
                
                if (NeedToReadLinks && Monitor.TryEnter(Manager.LinksLocker))
                {
                    Console.WriteLine("StReadLinks");
                    Manager.LinksMutex.WaitOne();
                    ReadFile("links.json");
                    Manager.LinksMutex.ReleaseMutex();
                    NeedToReadLinks = false;
                    Monitor.Exit(Manager.LinksLocker);
                    Console.WriteLine("EndReadLinks");
                }

                if (NeedToReadImages && Monitor.TryEnter(Manager.ImagesLocker))
                {
                    Console.WriteLine("StReadImages");
                    Manager.ImagesMutex.WaitOne();
                    ReadFile("images.json");
                    Manager.ImagesMutex.ReleaseMutex();
                    NeedToReadImages = false;
                    Monitor.Exit(Manager.ImagesLocker);
                    Console.WriteLine("EndReadImages");
                }
            }
            Console.WriteLine("EndReading");
        }

        private static void ReadFile(string filename)
        {
//            using (StreamWriter sw = new StreamWriter("../../" + filename, true, System.Text.Encoding.UTF8))
//            {
//                sw.WriteLine();
//                sw.Write("Здесь был " + filename);
//            }
            using (var sr = new StreamReader($@"C:\Users\Александр\Desktop\НЕ МОЕ\SeleniumApplications\jsons\{filename}"))
            {
                sr.ReadToEnd();
            }
        }
    }
}