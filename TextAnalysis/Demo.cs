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
            string wbrPath = Path.Combine(Cfg.DemoFolder, fileName);
            Common.RunWordBreak(Cfg.SimilarityInputPath, wbrPath, Cfg.PythonPath, Path.Combine(Cfg.PythonFolder, Constants.JiebaWbrExternalPath));

        }

        private IEnumerable<string> CalcSimilar(string inputTestDataPath,string simPath)
        {
            var simList = File.ReadLines(simPath).Select(x => new SimilarList(x)).ToArray();
            foreach(string testLine in File.ReadLines(inputTestDataPath))
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
                totalSum += groupSum;
                string groupResult = $"Similarity of group {n++}: {groupSum / group.Count()}.";
                resultList.Insert(0, groupResult);
                totalResultList.AddRange(resultList);
            }
            string overallResult = $"Overall similarity is: {totalSum / n}.";
            totalResultList.Insert(0, overallResult);
            return totalResultList;
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
            SimDict = split.Skip(1).ToDictionary(x => x.Split(' ')[0], x => double.Parse(x.Split(' ')[1]));
        }

        public Tuple<string,double> CalcSim(string[] words)
        {
            var same = SimDict.Keys.Intersect(words).ToArray();
            if (same.Length == 0)
                return new Tuple<string, double>("N/A", 0);
            return new Tuple<string, double>(same[0], SimDict[same[0]]);
        }
    }
}
