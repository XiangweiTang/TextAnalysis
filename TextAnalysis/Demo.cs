using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TextAnalysis
{
    class Demo
    {
        Config Cfg = new Config();
        public Demo(Config cfg)
        {
            Cfg = cfg;
        }
        public void RunSimilarity()
        {
            string fileName = Cfg.SimilarityInputPath.Split('\\').Last();
            string wbrPath = Path.Combine(Cfg.TmpPath, fileName);
            Common.RunWordBreak(Cfg.SimilarityInputPath, wbrPath, Cfg.PythonPath, Path.Combine(Cfg.PythonFolder, Constants.JiebaWbrExternalPath));
            var list = CalcSimilar(wbrPath, Cfg.DemoSimStandardPath);
            File.WriteAllLines(Cfg.SimilarityOutputPath, list);
        }

        private IEnumerable<string> CalcSimilar(string inputTestDataPath,string simPath)
        {
            var simList = File.ReadLines(simPath).Select(x => new SimilarList(x)).ToArray();
            foreach(string testLine in File.ReadLines(inputTestDataPath).Where(x=>!string.IsNullOrWhiteSpace(x)))
            {
                foreach (string line in GetSimilar(testLine.Split(' '), simList))
                    yield return line;
                yield return Constants.Divider;
            }
        }

        private IEnumerable<string> GetSimilar(string[] testData, IEnumerable<SimilarList> simList)
        {
            double totalSum = 0;
            List<string> totalResultList = new List<string>();
            int n = 0;
            foreach(var group in simList.GroupBy(x=>x.Id))
            {
                List<string> resultList = new List<string>();
                double groupSum = 0;                
                foreach(var sim in group)
                {
                    var simR = sim.CalcSim(testData);
                    string result = string.Join("\t", string.Empty, sim.StandardWord, simR.Item1, simR.Item2);
                    groupSum += simR.Item2;
                    resultList.Add(result);
                }
                totalSum += groupSum / group.Count();
                string groupResult = $"Similarity of group {n++}: {groupSum / group.Count()}.";
                resultList.Insert(0, groupResult);
                totalResultList.AddRange(resultList);
            }
            string overallResult = $"Overall similarity is: {totalSum / n}.";
            totalResultList.Insert(0, overallResult);
            return totalResultList;
        }
        
        public void RunClassification()
        {
            string testTextPath = Cfg.ClfInputPath;
            string name = Guid.NewGuid().ToString();
            string testText256Path = Path.Combine(Cfg.TmpPath, name + "_256.txt");
            string testDigitPath = Path.Combine(Cfg.TmpPath, name + "_digit.txt");
            string testPredictPath = Path.Combine(Cfg.TmpPath, name + "_tmp.txt");
            CreateTestDataWithProperLength(testTextPath, testText256Path);
            CreateTestData(testText256Path, testDigitPath);
            CalculateEvaluate(testDigitPath, testPredictPath, testTextPath, Cfg.ClfOutputPath);
        }

        private void CreateTestDataWithProperLength(string testTextPath, string testText256Path)
        {
            Common.RunWordBreak(testTextPath, testText256Path, Cfg.PythonPath, Path.Combine(Cfg.PythonFolder, Constants.JiebaWbrExternalPath));
            var list = File.ReadAllLines(testText256Path).Select(x => x.Split('\t'))
                .Select(x => x.ArraySkip(x.Length - 256))
                .Select(x => string.Join(" ", x))
                .Where(x=>!string.IsNullOrWhiteSpace(x))
                .ToList();
            File.WriteAllLines(testText256Path, list);
        }

        private void CreateTestData(string testText256Path, string testDigitPath)
        {
            Dictionary<string, string> t2dDict = File.ReadLines(Cfg.DemoClfDictPath).ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
            var list = File.ReadLines(testText256Path)
                .Select(x => x.Split(' '))
                .Select(x => x.Select(y => t2dDict.ContainsKey(y) ? t2dDict[y] : t2dDict[Constants.UNK]))
                .Select(x => string.Join(" ", x));
            File.WriteAllLines(testDigitPath, list);
        }

        private void CalculateEvaluate(string testDataPath, string predictPath, string testTextPath, string outputPath)
        {
            string scriptPath = Common.GetFullPath(Path.Combine(Cfg.DemoPythonFolder,Constants.PredictExternalPath));
            string digitPath = Common.GetFullPath(testDataPath);
            predictPath = Common.GetFullPath(predictPath);
            string pythonScriptPath = Path.Combine(Cfg.DemoPythonFolder, Constants.PredictExternalPath);
            Common.RunPredict(digitPath, predictPath, Cfg.PythonPath, scriptPath, Cfg.DemoPythonFolder);
            var list = File.ReadLines(predictPath).Zip(File.ReadLines(testTextPath), (x, y) => x + "\t" + y);
            File.WriteAllLines(outputPath, list);
        }
    }

    class SimilarList
    {
        public int Id { get; private set; } = -1;
        public string StandardWord { get; private set; } = string.Empty;
        public Dictionary<string, double> SimDict { get; private set; } = new Dictionary<string, double>();
        public SimilarList(string line)
        {
            var split = line.Split('\t');
            Id = int.Parse(split[0]);
            StandardWord = split[1];
            SimDict = split.Skip(2).ToDictionary(x => x.Split(' ')[0], x => double.Parse(x.Split(' ')[1]));
        }

        public Tuple<string,double> CalcSim(string[] words)
        {
            if (words.Contains(StandardWord))
                return new Tuple<string, double>(StandardWord, 1);
            var same = SimDict.Keys.Intersect(words).ToArray();
            if (same.Length == 0)
                return new Tuple<string, double>("N/A", 0);
            return new Tuple<string, double>(same[0], SimDict[same[0]]);
        }
    }
}
