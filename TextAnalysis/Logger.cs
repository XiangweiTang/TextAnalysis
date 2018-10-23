using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    static class Logger
    {
        public static string LogPath = string.Empty;
        
        public static void WriteLine(string content)
        {
            using(StreamWriter sw=new StreamWriter(LogPath))
            {
                string line = DateTime.Now.ToLongTimeString() + "\t" + content;
                Console.WriteLine(line);
                sw.WriteLine(line);
            }
        }
    }
}
