using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TextAnalysis
{
    class Word2VecCalcSimilarity
    {
        Config Cfg = new Config();
        public string TmpName = string.Empty;
        IEnumerable<Similarity> SimList = Enumerable.Empty<Similarity>();
        List<string> BriefList = new List<string>();
        public Word2VecCalcSimilarity(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            DataProcessing dataProcessing = new DataProcessing(Cfg);
            string prePath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.post");

            dataProcessing.PreProcessFile(Cfg.SimilarityTestPath, prePath);
            dataProcessing.WordBreakFile(prePath, wbrPath);
            dataProcessing.PostProcessFile(wbrPath, postPath);

            PrepareSimilarity();

            var resultList = File.ReadLines(postPath).SelectMany(x => ProcessSingleLine(x));
            
            File.WriteAllLines(Cfg.SimDetailPath, resultList);
            File.WriteAllLines(Cfg.SimBriefPath, BriefList);
        }


        private IEnumerable<string> ProcessSingleLine(string line)
        {
            var split = line.Split(' ');
            var results = CalcSimilarity(split).ToArray();
            double totalAvg = results.Average(x => x.Score);
            BriefList.Add(totalAvg + "\t" + line);
            return OutputResult(results);
        }

        private IEnumerable<SimResult> CalcSimilarity(string[] wordList)
        {
            foreach(Similarity sim in SimList)
            {
                var common = sim.SimDict.Keys.Intersect(wordList).ToArray();
                if (common.Count() == 0)
                    yield return new SimResult(sim.GroupId, sim.SimDict.First().Key, "<NA>", 0);
                else
                    yield return new SimResult(sim.GroupId, sim.SimDict.First().Key, common[0], sim.SimDict[common[0]]);                        
            }
        }

        private void PrepareSimilarity()
        {
            var keywordList = File.ReadLines(Cfg.SimilarityKeyWordPath);
            var valueList = File.ReadLines(Cfg.Word2VecSimilarityPath);
            SimList = keywordList.Zip(valueList, (x, y) => new Similarity(x, y)).ToList();
        }

        private IEnumerable<string> OutputResult(SimResult[] results)
        {
            double totalAvg = results.Average(x => x.Score);
            yield return $"The overall similarity is: {totalAvg}.";
            yield return string.Empty;
            var groups = results.GroupBy(x => x.GroupId);
            foreach(var group in groups)
            {
                double currentAvg = group.Average(x => x.Score);
                yield return $"The similarity of group {group.Key} is: {currentAvg}.";
                foreach(SimResult sResult in group)
                {
                    yield return $"\t{sResult.Output}";
                }
            }
            yield return string.Empty;
        }

        class Similarity
        {
            public string GroupId { get; private set; } = "Default";
            public Dictionary<string, double> SimDict =>
                ValueLine.Split('\t').ToDictionary(x => x.Split(' ')[0], x => double.Parse(x.Split(' ')[1]));
            private string ValueLine = string.Empty;
            public Similarity(string keywordLine, string valueLine)
            {
                GroupId = keywordLine.Split('\t')[0];
                ValueLine = valueLine;
            }
        }

        class SimResult
        {
            public string GroupId { get; }
            public string KeyWord { get; }
            public string MatchWord { get; }
            public double Score { get; }
            public SimResult(string groupId, string keyWord, string matchWord, double score)
            {
                GroupId = groupId;
                KeyWord = keyWord;
                MatchWord = matchWord;
                Score = score;
            }

            public string Output => string.Join(" ", KeyWord, MatchWord, Score);
        }
    }
}
