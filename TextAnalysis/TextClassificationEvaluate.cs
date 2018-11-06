using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class TextClassificationEvaluate
    {
        Config Cfg = new Config();
        public TextClassificationEvaluate(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            var batches = File.ReadLines(Cfg.UsedDataFile)
                .Where(x => x[0] != '#')
                .Select(x => x.Split('\t'))
                .Where(x => x[0] == Constants.SUP)
                .Select(x => x[1]);
            var resultList = batches.Select(x => Run(x)).ToList();
            foreach(string result in resultList)
                Console.WriteLine(result);
            File.WriteAllLines(Cfg.TextClassificationEvaluationResultPath, resultList);
        }

        public string Run(string batchName)
        {
            string dataPath = Path.Combine(Cfg.SupDataFolder, $"{batchName}.{Constants.TEST}.{Cfg.Locale}.txt");
            string labelPath = Path.Combine(Cfg.SupLabelFolder, $"{batchName}.{Constants.TEST}.{Cfg.Locale}.txt");
            string outputPath = Path.Combine(Cfg.TmpFolder, $"{batchName}.{Constants.TEST}.{Cfg.Locale}.txt");
            string args = string.Join(" ", Cfg.EvaluateScriptPath, dataPath, labelPath, Cfg.TextClassificationModelPath, outputPath);
            Common.RunFile(Cfg.PythonPath, args);
            string score = File.ReadAllText(outputPath);
            return score + "\t" + batchName;
        }
    }
}
