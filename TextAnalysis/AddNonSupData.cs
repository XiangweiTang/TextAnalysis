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

            Digitalize digit = new Digitalize(Cfg);
            digit.Run();

            string dataStatus = string.Join("\t", Constants.NONSUP, Cfg.BatchName, Cfg.DataDescription);
            File.AppendAllLines(Cfg.UsedDataFile, new List<string> { dataStatus });            
        }

        private void Cleanup()
        {
            var list = Directory.EnumerateFiles(Cfg.NonlabeledFolder).SelectMany(x => File.ReadLines(x));
            string rawPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.raw");
            string preProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postProcessPath = Path.Combine(Cfg.NonSupTextFolder, $"{Cfg.BatchName}.{Cfg.Locale}.txt");
            DataProcessing dataProcessing = new DataProcessing(Cfg);
            File.WriteAllLines(rawPath, list);
            dataProcessing.PreProcessFile(rawPath, preProcessPath);
            dataProcessing.WordBreakFile(preProcessPath, wbrPath);
            dataProcessing.PostProcessFile(wbrPath, postProcessPath);
        }
    }
}
