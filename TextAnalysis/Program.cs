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
            Config cfg = new Config();
            RunTextAnalysis rta = new RunTextAnalysis(cfg);
            rta.Run();
        }
    }
}
