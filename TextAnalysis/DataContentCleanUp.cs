using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TextAnalysis
{
    class DataContentCleanUp
    {
        Config Cfg = new Config();
        public int DevCount { get; private set; } = 0;
        public int TestCount { get; private set; } = 0;
        public int TrainCount { get; private set; } = 0;
        private List<string> NewFileList = new List<string>();
        
        public DataContentCleanUp(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            Logger.WriteLine("Data process starts.");
            Logger.WriteLine("Data process is done.");
        }


        private void DataCollect()
        {
            Console.WriteLine("Start to collect data.");
            foreach(string filePath in Directory.EnumerateFiles(Cfg.InputFolder, "*", SearchOption.AllDirectories))
            {
                string fileName = Guid.NewGuid().ToString() + ".txt";
                NewFileList.Add(fileName);
                string outputFilePath = Path.Combine(Cfg.RawFolder, fileName);
                File.Copy(filePath, outputFilePath);
                string mapping = filePath + "\t" + fileName;
                Logger.WriteLine(mapping);
            }
            Console.WriteLine("\tDone");
        }

        private void PreProcess()
        {
            Console.WriteLine("Run pre-process.");
            string inputFolder = Cfg.RawFolder;
            string outputFolder = Cfg.PreProcessFolder;
            Common.FolderTransport(inputFolder, outputFolder, PreProcess);
            Console.WriteLine("\tDone");
        }

        private void PreProcess(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine("Run pre-process");
            var list = File.ReadLines(inputFilePath)
                .Select(x => CleanupLineChars(x));
            File.WriteAllLines(outputFilePath, list);
        }

        private string CleanupLineChars(string inputLine)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in inputLine)
            {
                if (Cfg.KeptIntervals.Any(x => (x.Item1 <= c) && (x.Item2 >= c)))
                    sb.Append(c);
            }
            return sb.ToString();
        }
        

        private void WordBreak()
        {
            Console.WriteLine("Run word break.");
            Common.ProcessFile pf;
            switch (Cfg.WordBreakType)
            {
                case "jieba":
                    pf = RunJiebaWordBreak;
                    break;
                default:
                    pf = File.Copy;
                    break;
            }
            string inputFolder = Cfg.PreProcessFolder;
            string outputFolder = Cfg.WordBreakFolder;
            Common.FolderTransport(inputFolder, outputFolder, pf);
            Console.WriteLine("\tDone");
        }

        private void RunJiebaWordBreak(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine("Processing " + inputFilePath);
            string pythonPath = Cfg.PythonPath;
            string scriptPath = Path.Combine(Cfg.PythonFolder, Constants.JiebaWbrInternalPath);
            Common.RunWordBreak(inputFilePath, outputFilePath, pythonPath, scriptPath);                        
        }


        private void PostProcess()
        {
            Console.WriteLine("Run post-process...");
            string inputFolder = Cfg.WordBreakFolder;
            string outputFolder = Cfg.PostProcessFolder;
            Common.FolderTransport(inputFolder, outputFolder, PostProcess);
            Console.WriteLine("\tDone.");
        }

        private void PostProcess(string inputFilePath, string outputFilePath)
        {
            var list = File.ReadLines(inputFilePath)
                .Select(x => CleanupLineSpaces(x));
            File.WriteAllLines(outputFilePath, list);
        }

        private string CleanupLineSpaces(string inputLine)
        {
            return Regex.Replace(inputLine, "\\s+", " ").Trim();
        }


        private void TrainDevTestProcess(string inputPath, string outputPath)
        {
            Console.WriteLine("Run train/dev/test process.");
            var list = Directory.EnumerateFiles(inputPath).SelectMany(x => File.ReadLines(x));
            var randomArray = Common.Shuffle(list);
            int total = randomArray.Length;
            SetCount(total);
            var devList = randomArray.ArrayTake(DevCount);
            var testList = randomArray.ArrayRange(DevCount, TestCount);
            var trainList = randomArray.ArraySkip(DevCount + TestCount);
            if(devList.Length!=0)
            {
                string devPath = Path.Combine(outputPath, Cfg.DataLabel + "_dev.txt");
                File.WriteAllLines(devPath, devList);
            }
            if (testList.Length != 0)
            {
                string testPath = Path.Combine(outputPath, Cfg.DataLabel + "_test.txt");
                File.WriteAllLines(testPath, testList);
            }
            if (trainList.Length != 0)
            {
                string trainPath = Path.Combine(outputPath, Cfg.DataLabel + "_train.txt");
                File.WriteAllLines(trainPath, trainList);
            }
            Console.WriteLine("\tDone.");
        }

        private void SetCount(int total)
        {
            if (Cfg.UseCount)
            {
                TestCount = Math.Max(Cfg.TestCount, 0);
                DevCount = Math.Max(Cfg.DevCount, 0);
            }
            else
            {
                TestCount = Math.Max(Convert.ToInt32(total * Cfg.TestRatio), Cfg.TestCount > 0 ? 1 : 0);
                DevCount = Math.Max(Convert.ToInt32(total * Cfg.DevRatio), Cfg.DevCount > 0 ? 1 : 0);
            }
            TrainCount = Math.Max(0, total - TestCount - DevCount);
        }
        
        
        private void UpdateDict()
        {
            Console.WriteLine("Update dict...");
            if (TrainCount == 0 && !Cfg.ForceUpdate)
                return;
            var list = new List<string> { "UNK\t0", "PAD\t1" }.Concat(
                Directory.EnumerateFiles(Cfg.TrainTextFolder).SelectMany(x => File.ReadLines(x)).SelectMany(x => x.Split(' '))
                .GroupBy(x => x)
                .OrderBy(x => x.Count())
                .Take(Cfg.MaxVocab)
                .Select((x, y) => x.Key + "\t" + (y + 2)));
            File.WriteAllLines(Cfg.DictPath, list);
            UpdateDigit();
        }

        private void UpdateDigit()
        {
            var dict = File.ReadLines(Cfg.DictPath).ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
            UpdateDigit(Cfg.TrainFolder, dict);
            UpdateDigit(Cfg.TestFolder, dict);
            UpdateDigit(Cfg.DevFolder, dict);
        }

        private void UpdateDigit(string folderPath, Dictionary<string,string> dict)
        {
            foreach(string filePath in Directory.EnumerateFiles(folderPath))
            {
                var list = File.ReadAllLines(filePath).Select(x => UpdateLineDigit(x, dict));
                File.WriteAllLines(filePath, list);
            }
        }

        private string UpdateLineDigit(string line, Dictionary<string,string> dict)
        {
            List<string> list = new List<string>();
            foreach(string word in line.Split(' '))
            {
                if (dict.ContainsKey(word))
                    list.Add(dict[word]);
                else
                    list.Add(dict["UNK"]);
            }
            return string.Join(" ", list);
        }
    }
}
