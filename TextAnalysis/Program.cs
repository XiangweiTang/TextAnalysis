using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = args.Length == 0 ? "Config.xml" : args[0];
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Config is missing.");
            }
            else
            {
                LoadConfigAndRun(configPath);
            }
        }

        static void LoadConfigAndRun(string configPath)
        {
            Config cfg = new Config();
            cfg.LoadConfig(configPath);
            RunTextAnalysis rta = new RunTextAnalysis(cfg);
            rta.Run();
        }
    }
}
