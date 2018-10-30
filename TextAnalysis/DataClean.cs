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
        public DataClean(Config cfg)
        {
            Cfg = cfg;
            TmpName = new Guid().ToString();
        }


        private void ProcessFile()
        {

        }

        private void ToDigit(string inputPath, string outputPath, Dictionary<string,string> dict)
        {
            var list = File.ReadLines(inputPath)
                .Select(x => x.Split(Sep, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.ArraySkip(x.Length - 256))
                .Select(x => string.Join(" ", x.Select(y => dict.ContainsKey(y) ? dict[y] : dict[Constants.UNK])));
            File.WriteAllLines(outputPath, list);
        }

        private void MergeData()
        {
            File.AppendAllText(Cfg.FileMappingPath, Cfg.BatchName + "\t" + TmpName);
            if (Directory.Exists(Cfg.PositiveFolder))
                PreCleanupLabeledFiles(Cfg.PositiveFolder, "Pos");
            if (Directory.Exists(Cfg.NegativeFolder))
                PreCleanupLabeledFiles(Cfg.NegativeFolder, "Neg");

            string posPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Pos_wbr.txt");
            string negPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_Neg_wbr.txt");
            MergeAndSplit(posPath, negPath);


        }



        private void CreateDict()
        {
            string dictPath = Path.Combine(Cfg.DataFolder, Cfg.Locale + ".dict");
            var list = Directory.EnumerateFiles(Cfg.NonLabelTextFolder)
                .SelectMany(x => File.ReadLines(x))
                .SelectMany(x => x.Split(Sep, StringSplitOptions.RemoveEmptyEntries));
            var groups = list.GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key);
            int n = 0;
            var dict = Constants.Kept.Concat(groups).ToDictionary(x => x, x => n++);
            File.WriteAllLines(dictPath, dict.Select(x => x.Key + "\t" + x.Value));
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

            string devPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.DEV}.txt");
            var dev = labelList.ArrayTake(devCount);
            File.WriteAllLines(devPath, dev);

            string testPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.TEST}.txt");
            var test = labelList.ArrayRange(devCount, testCount);
            File.WriteAllLines(testPath, test);

            string trainPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{Constants.TRAIN}.txt");
            var train = labelList.ArraySkip(devCount + testCount);
            File.WriteAllLines(trainPath, train);
        }

        private void PreCleanupLabeledFiles(string folderPath, string label)
        {            
            string cleanPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}_{label}_clean.txt");
            PreCleanUp(folderPath, cleanPath);

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
        private void PreCleanUp(string folderPath,string preCleanPath)
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
