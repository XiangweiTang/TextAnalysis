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
            TextPreProcessing tpp = new TextPreProcessing(cfg);
            //tpp.Run();

            TextToDigit ttd = new TextToDigit(cfg);
            ttd.Run();
        }
    }
}
