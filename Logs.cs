using System;
using System.IO;
using System.Text;

namespace VK2Skype
{
    public static class Logs
    {
        private const string iis = @"C:\inetpub\wwwroot\iisstart.htm";

        static Logs()
        {
            if (!(File.Exists("log.txt")))
            {
                File.Create("log.txt");
            }

            if (!(File.Exists("errorlog.txt")))
            {
                File.Create("errorlog.txt");
            }
        }

        public static void WriteLog(string file, string logstring)
        {
            Console.WriteLine(logstring);

            File.AppendAllText(File.Exists(iis) 
                ? iis 
                : file, 
                string.Format("{0} => {1}\n", DateTime.Now, logstring), Encoding.UTF8);
        }
    }
}
