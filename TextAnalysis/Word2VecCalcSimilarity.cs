using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Word2VecCalcSimilarity
    {
        Config Cfg = new Config();
        public string TmpName = string.Empty;
        public Word2VecCalcSimilarity(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            DataProcessing dataProcessing = new DataProcessing(Cfg);
            string prePath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.post");

            dataProcessing.PreProcessFile(Cfg.Word2VecTestPath, prePath);
            dataProcessing.WordBreakFile(prePath, wbrPath);
            dataProcessing.PostProcessFile(wbrPath, postPath);
        }

        
    }
}
