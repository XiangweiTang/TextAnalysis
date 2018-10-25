using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = args.Length > 0 ? args[0] : "config.xml";
            Config cfg = new Config();
            Init init = new Init(cfg);
            cfg.LoadConfig(configPath);
            Demo d = new Demo(cfg);
            if (cfg.RunClf)
                d.RunClassification();
            if (cfg.RunSimilarity)
                d.RunSimilarity();
        }
    }
}
