using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class CalcSimilarity
    {
        Config Cfg = new Config();
        public string TmpName = string.Empty;
        public CalcSimilarity(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            NewDataProcessing ndp = new NewDataProcessing(Cfg);
            string prePath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.post");

            ndp.PreProcessFile(Cfg.Word2VecTestPath, prePath);
            ndp.WordBreakFile(prePath, wbrPath);
            ndp.PostProcessFile(wbrPath, postPath);
        }

        
    }
}
