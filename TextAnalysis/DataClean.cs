using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace TextAnalysis
{
    class DataClean
    {
        private Config Cfg = new Config();
        Regex LinkReg = new Regex("http[^\\s]*");
        Regex SpaceReg = new Regex("\\s+");
        string TmpName = string.Empty;
        private readonly char[] Sep = { ' ' };
        Dictionary<string, string> Dict = new Dictionary<string, string>();
        public string NonLabeledPath { get => Path.Combine(Cfg.NonLabelTextFolder, $"{Cfg.BatchName}.txt"); }        
        public string DevTextPath { get => Path.Combine(Cfg.TextFolder, $"{Cfg.BatchName}_{Constants.DEV}.txt"); }
        public string DevLabelPath { get => Path.Combine(Cfg.LabelFolder, $"{Cfg.BatchName}_{Constants.DEV}.txt"); }
        string DevTmpPath { get => Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.DEV}.txt"); }
        public string TestTextPath { get => Path.Combine(Cfg.TextFolder, $"{Cfg.BatchName}_{Constants.TEST}.txt"); }
        public string TestLabelPath { get => Path.Combine(Cfg.LabelFolder, $"{Cfg.BatchName}_{Constants.TEST}.txt"); }
        string TestTmpPath { get => Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.TEST}.txt"); }
        public string TrainTextPath { get => Path.Combine(Cfg.TextFolder, $"{Cfg.BatchName}_{Constants.TRAIN}.txt"); }
        public string TrainLabelPath { get => Path.Combine(Cfg.LabelFolder, $"{Cfg.BatchName}_{Constants.TRAIN}.txt"); }
        string TrainTmpPath { get => Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.TRAIN}.txt"); }
        public DataClean(Config cfg)
        {
            Cfg = cfg;
            TmpName = new Guid().ToString();
        }
        public void Run()
        {
            if (Cfg.AddLabeledData)
                ProcessLabeledData();
            if (Cfg.AddNonLabeledData)
                ProcessNonLabeledData();
            if (Cfg.RebuildDict)
                RebuildDict();
            ToDigit();
            File.AppendAllText(Cfg.UsedDataFile, string.Join("\t", Cfg.BatchName, Cfg.DataDescription));
        }       
        private void ToDigit()
        {
            Dict = File.ReadLines(Cfg.DictPath).ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
            Common.FolderTransport(Cfg.TextFolder, Cfg.DigitFolder, ToDigit);
        }
        private void ToDigit(string inputPath, string outputPath)
        {
            var list = File.ReadLines(inputPath)
                .Select(x => x.Split(Sep, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.ArraySkip(x.Length - 256))
                .Select(x => string.Join(" ", x.Select(y => Dict.ContainsKey(y) ? Dict[y] : Dict[Constants.UNK])));
            File.WriteAllLines(outputPath, list);
        }
        private void ProcessNonLabeledData()
        {
            File.AppendAllText(Cfg.FileMappingPath, Cfg.BatchName + "\t" + TmpName);
            if (Directory.Exists(Cfg.NonlabeledFolder))
                PreCleanupNonlabledFiles(Cfg.NonlabeledFolder);

            string cleanPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_clean.txt");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_wbr.txt");
            Common.RunWordBreak(cleanPath, wbrPath, Cfg.PythonPath, Cfg.JiebaScriptPath);

            TransportNonLabeledData();
        }
        private void PreCleanupNonlabledFiles(string folderPath)
        {
            string cleanPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_clean.txt");
            PreCleanup(folderPath, cleanPath);

            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_wbr.txt");
            WordBreak(cleanPath, wbrPath);
        }
        private void TransportNonLabeledData()
        {
            var list = File.ReadLines(Path.Combine(Cfg.TmpFolder, $"{TmpName}_wbr.txt"))
                .Select(x => SpaceReg.Replace(x, " ").Trim());
            File.WriteAllLines(NonLabeledPath, list);            
        }
        private void ProcessLabeledData()
        {
            File.AppendAllText(Cfg.FileMappingPath, Cfg.BatchName + "\t" + TmpName);
            if (Directory.Exists(Cfg.PositiveFolder))
                PreCleanupLabeledFiles(Cfg.PositiveFolder, "Pos");
            if (Directory.Exists(Cfg.NegativeFolder))
                PreCleanupLabeledFiles(Cfg.NegativeFolder, "Neg");

            string posPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Pos_wbr.txt");
            string negPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Neg_wbr.txt");
            MergeAndSplit(posPath, negPath);

            TransportLabeledData();
        }
        private void TransportLabeledData()
        {
            if (File.Exists(DevTmpPath))
                SplitLabel(DevTmpPath, DevTextPath, DevLabelPath);
            if (File.Exists(TestTmpPath))
                SplitLabel(TestTmpPath, TestTextPath, TestLabelPath);
            if (File.Exists(TrainTmpPath))
                SplitLabel(TrainTmpPath, TrainTextPath, TrainLabelPath);
        }
        private void SplitLabel(string srcPath, string dstDataPath, string dstLabelPath)
        {
            var list = File.ReadAllLines(srcPath);
            var dataList = list.Select(x => x.Split('\t')[0]);
            var labelList = list.Select(x => SpaceReg.Replace(x.Split('\t')[1], " ").Trim());
            File.WriteAllLines(dstDataPath, dataList);
            File.WriteAllLines(dstLabelPath, labelList);
        }
        private void RebuildDict()
        {
            var list = Directory.EnumerateFiles(Cfg.NonLabelTextFolder)
                .SelectMany(x => File.ReadLines(x))
                .SelectMany(x => x.Split(Sep, StringSplitOptions.RemoveEmptyEntries));
            var groups = list.GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key);
            int n = 0;
            var dict = Constants.Kept.Concat(groups).ToDictionary(x => x, x => n++);
            File.WriteAllLines(Cfg.DictPath, dict.Select(x => x.Key + "\t" + x.Value));
        }
        private void MergeAndSplit(string posPath, string negPath)
        {
            var labelList = File.ReadLines(posPath).Select(x => "1\t" + x)
                .Concat(File.ReadLines(negPath).Select(x => "0\t" + x))
                .Shuffle();
            var nonLabellist = labelList.Select(x => x.Split('\t')[1]);

            string nonLabelPath = Path.Combine(Cfg.NonLabelTextFolder, $"{Cfg.BatchName}.{Cfg.Locale}.txt");
            File.WriteAllLines(nonLabelPath, nonLabellist);

            int total = labelList.Length;
            int devCount = Convert.ToInt32(Cfg.DevRate * total);
            int testCount = Convert.ToInt32(Cfg.TestRate * total);

            if (devCount > 0)
            {
                var dev = labelList.ArrayTake(devCount);
                File.WriteAllLines(DevTmpPath, dev);
            }
            if (testCount > 0)
            {
                var test = labelList.ArrayRange(devCount, testCount);
                File.WriteAllLines(TestTmpPath, test);
            }
                        
            if (total - devCount - testCount > 0)
            {
                var train = labelList.ArraySkip(devCount + testCount);
                File.WriteAllLines(TrainTmpPath, train);
            }
        }
        private void PreCleanupLabeledFiles(string folderPath, string label)
        {            
            string cleanPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{label}_clean.txt");
            PreCleanup(folderPath, cleanPath);

            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{label}_wbr.txt");
            WordBreak(cleanPath, wbrPath);            
        }
        private void WordBreak(string inputFilePath, string outputFilePath)
        {
            if (Cfg.Locale == "CHS")
            {
                Common.RunWordBreak(inputFilePath, outputFilePath, Cfg.PythonPath, Cfg.JiebaScriptPath);
            }
            else
            {
                File.Copy(inputFilePath, outputFilePath);
            }
        }
        private void PreCleanup(string folderPath,string preCleanPath)
        {
            var preCleanList = Directory.EnumerateFiles(folderPath).SelectMany(x => File.ReadLines(x))
                .Select(x => PreCleanup(x));
            File.WriteAllLines(preCleanPath, preCleanList);
        }
        private string PreCleanup(string line)
        {
            string lower = line.ToLower();
            string noLinkStr = LinkReg.Replace(lower, " ");
            string charClean = new string(CleanupChars(noLinkStr).ToArray());
            string spaceClean = SpaceReg.Replace(charClean, " ").Trim();
            string outputString = spaceClean;
            return outputString;
        }
        private IEnumerable<char> CleanupChars(string line)
        {
            foreach(char c in line)
            {
                foreach(string interval in Cfg.ValidIntervals)
                {
                    if(interval[0]<=c&&c<=interval[1])
                    {
                        yield return c;
                        break;
                    }
                }
            }
        }
    }
}
