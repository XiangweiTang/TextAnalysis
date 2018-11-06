using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class AddSupData
    {
        Config Cfg = new Config();
        string TmpName = string.Empty;
        Dictionary<string, string> WordToDigitDict = new Dictionary<string, string>();
        public AddSupData(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            string posPath = Cleanup(Cfg.PositiveFolder, "Pos");
            string negPath = Cleanup(Cfg.NegativeFolder, "Neg");
            MergeData(posPath, negPath);

            Digitalize dgt = new Digitalize(Cfg);
            Common.FolderTransport(Cfg.SupTextFolder, Cfg.SupDataFolder, dgt.Run);

            string dataStatus = string.Join("\t", Constants.SUP, Cfg.BatchName, Cfg.DataDescription);
            File.AppendAllLines(Cfg.UsedDataFile, new List<string> { dataStatus });
        }

        private string Cleanup(string inputFolder, string type)
        {
            var rawList = Directory.EnumerateFiles(inputFolder).SelectMany(x => File.ReadLines(x));
            string rawPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{type}.{Cfg.Locale}.raw");
            string preProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{type}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{type}.{Cfg.Locale}.wbr");
            string postProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{type}.{Cfg.Locale}.post");
            DataProcessing dataProcessing = new DataProcessing(Cfg);
            File.WriteAllLines(rawPath, rawList);
            dataProcessing.PreProcessFile(rawPath, preProcessPath);
            dataProcessing.WordBreakFile(preProcessPath, wbrPath);
            dataProcessing.PostProcessFile(wbrPath, postProcessPath);
            return postProcessPath;
        }

        private void MergeData(string posPath, string negPath)
        {
            var posList = File.ReadLines(posPath).Select(x => new { Label = "1", Text = x });
            var negList = File.ReadLines(negPath).Select(x => new { Label = "0", Text = x });
            var array = posList.Concat(negList).Shuffle();
            int total = array.Length;
            int testCount = Convert.ToInt32(total * Cfg.TestRate);
            int devCount = Convert.ToInt32(total * Cfg.DevRate);
            int trainCount = total - devCount - testCount;
            var test = array.ArrayTake(testCount);
            var dev = array.ArrayRange(testCount, devCount);
            var train = array.ArraySkip(testCount + devCount);
            Split(test.Select(x => x.Text), test.Select(x => x.Label), Constants.TEST);
            Split(dev.Select(x => x.Text), test.Select(x => x.Label), Constants.DEV);
            Split(train.Select(x => x.Text), train.Select(x => x.Label), Constants.TRAIN);
        }

        private void Split(IEnumerable<string> textList, IEnumerable<string> labelList, string type)
        {
            string textPath = Path.Combine(Cfg.SupTextFolder, $"{Cfg.BatchName}.{type}.{Cfg.Locale}.txt");
            string labelPath = Path.Combine(Cfg.SupLabelFolder, $"{Cfg.BatchName}.{type}.{Cfg.Locale}.txt");
            File.WriteAllLines(textPath, textList);
            File.WriteAllLines(labelPath, labelList);
        }
    }
}
