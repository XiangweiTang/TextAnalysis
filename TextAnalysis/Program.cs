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
            string configPath = args[0];
            Config cfg = new Config();
            cfg.LoadConfig(configPath);
            Demo d = new Demo(cfg);
            if (cfg.RunClf)
                d.RunClassification();
            if (cfg.RunSimilarity)
                d.RunSimilarity();
        }
    }
}
