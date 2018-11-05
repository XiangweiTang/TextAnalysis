using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class AddNonSupData
    {
        Config Cfg = new Config();
        string TmpName = string.Empty;
        public AddNonSupData(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            Cleanup();
            var fileList = Directory.EnumerateFiles(Cfg.NonSupTextFolder, $"*.{Cfg.Locale}.txt");
            Common.RebuildDictionary(fileList, Cfg.DictPath, Cfg.MaxVocab);
        }

        private void Cleanup()
        {
            var list = Directory.EnumerateFiles(Cfg.NonlabeledFolder).SelectMany(x => File.ReadLines(x));
            string rawPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_raw.txt");
            string preProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_pre.txt");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_wbr.txt");
            string postProcessPath = Path.Combine(Cfg.NonSupTextFolder, $"{Cfg.BatchName}.{Cfg.Locale}.txt");
            NewDataProcessing ndp = new NewDataProcessing(Cfg);
            File.WriteAllLines(rawPath, list);
            ndp.PreProcessFile(rawPath, preProcessPath);
            ndp.WordBreakFile(preProcessPath, wbrPath);
            ndp.PostProcessFile(wbrPath, postProcessPath);
        }
    }
}
