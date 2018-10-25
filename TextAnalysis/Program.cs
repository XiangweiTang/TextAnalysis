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
            Config cfg = new Config();
            Init init = new Init(cfg);
            Demo d = new Demo(cfg);
            d.RunClassification();
            d.RunSimilarity();
        }
    }
}
