using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AAAService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AAAService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
    public static class Helper
    {
        public static string ToStr(this IEnumerable<string> source, string separator = ", ") => String.Join(separator, source);

        public static T DeserializeTo<T>(this string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(fs);
            }
        }
    }
}
