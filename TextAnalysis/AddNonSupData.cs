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
            string dataStatus = string.Join("\t", Constants.NONSUP, Cfg.BatchName, Cfg.DataDescription);
            File.AppendAllText(Cfg.UsedDataFile, dataStatus);
        }

        private void Cleanup()
        {
            var list = Directory.EnumerateFiles(Cfg.NonlabeledFolder).SelectMany(x => File.ReadLines(x));
            string rawPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.raw");
            string preProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postProcessPath = Path.Combine(Cfg.NonSupTextFolder, $"{Cfg.BatchName}.{Cfg.Locale}.txt");
            NewDataProcessing ndp = new NewDataProcessing(Cfg);
            File.WriteAllLines(rawPath, list);
            ndp.PreProcessFile(rawPath, preProcessPath);
            ndp.WordBreakFile(preProcessPath, wbrPath);
            ndp.PostProcessFile(wbrPath, postProcessPath);
        }
    }
}
