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
            Cleanup(Cfg.PositiveFolder, "Pos");
            Cleanup(Cfg.NegativeFolder, "Neg");
            MergePosNeg("dev");
            MergePosNeg("test");
            MergePosNeg("train");

            var fileList = Directory.EnumerateFiles(Cfg.NonSupTextFolder, $"*.{Cfg.Locale}.txt");
            Common.RebuildDictionary(fileList, Cfg.DictPath, Cfg.MaxVocab);
            WordToDigitDict = File.ReadLines(Cfg.DictPath).ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
            Common.FolderTransport(Cfg.SupTextFolder, Cfg.SupDigitFolder,TextToDigit);
        }

        private void Cleanup(string inputFolder, string type)
        {
            var rawList = Directory.EnumerateFiles(inputFolder).SelectMany(x => File.ReadLines(x));
            string rawPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_raw.txt");
            string preProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_pre.txt");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_wbr.txt");
            string postProcessPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_post.txt");
            NewDataProcessing ndp = new NewDataProcessing(Cfg);
            File.WriteAllLines(rawPath, rawList);
            ndp.PreProcessFile(rawPath, preProcessPath);
            ndp.WordBreakFile(preProcessPath, wbrPath);
            ndp.PostProcessFile(wbrPath, postProcessPath);

            var cleanList = File.ReadLines(postProcessPath).Shuffle();
            int total = cleanList.Length;
            int testCount = Convert.ToInt32(Cfg.TestRate * total);
            int devCount = Convert.ToInt32(Cfg.DevRate * total);
            int trainCount = total - testCount - devCount;
            var dev = cleanList.ArrayTake(devCount);
            var test = cleanList.ArrayRange(devCount, testCount);
            var train = cleanList.ArraySkip(devCount + testCount);
            string devPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_dev.txt");
            string testPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_test.txt");
            string trainPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{type}_train.txt");
            File.WriteAllLines(devPath, dev);
            File.WriteAllLines(testPath, test);
            File.WriteAllLines(trainPath, train);
        }

        private void MergePosNeg(string dataType)
        {
            string posPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Pos_{dataType}.txt");
            string negPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Neg_{dataType}.txt");
            string outputNonSupTextPath = Path.Combine(Cfg.NonSupTextFolder, $"{Cfg.BatchName}_{dataType}.txt");
            string outputSupTextPath = Path.Combine(Cfg.SupTextFolder, $"{Cfg.BatchName}_{dataType}.txt");
            string outputLabelPath = Path.Combine(Cfg.SupLabelFolder, $"{Cfg.BatchName}_{dataType}.txt");
            MergePosNeg(posPath, negPath, outputNonSupTextPath, outputLabelPath);
            File.Copy(outputNonSupTextPath, outputSupTextPath);
        }

        private void MergePosNeg(string posPath, string negPath, string outputDataPath, string outputLabelPath)
        {
            var posList = File.ReadLines(posPath).Select(x => new { Label = "1", Data = x });
            var negList = File.ReadLines(negPath).Select(x => new { Label = "0", Data = x });
            var list = posList.Concat(negList).Shuffle();
            File.WriteAllLines(outputDataPath, list.Select(x => x.Data));
            File.WriteAllLines(outputLabelPath, list.Select(x => x.Label));
        }
        private void TextToDigit(string wordPath, string digitPath)
        {
            var list = File.ReadLines(wordPath)
                .Select(x => x
                .Split(' ')
                .ArraySkip(256 - x.Length)
                .Select(y => WordToDigitDict.ContainsKey(y) ? WordToDigitDict[y] : WordToDigitDict[Constants.UNK]))
                .Select(x => string.Join(" ", x));
            File.WriteAllLines(digitPath, list);
        }
    }
}
