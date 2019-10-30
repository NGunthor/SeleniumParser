using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using MyLib;
using OpenQA.Selenium.Chrome;

namespace VKApp
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initialization...");
            var driver = new ChromeDriver();
            var manager = new Manager(driver,"http://vk.com");

            Console.WriteLine("Start");

            var threads = new ThreadManager(manager);

            while (threads.Threads[3].IsAlive) 
            { }
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
    public static class Helper
    {
        public static string ToStr(this IEnumerable<string> source, string separator = ", ") => string.Join(separator, source);

        public static T DeserializeTo<T>(this string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(fs);
            }
        }
    }
}