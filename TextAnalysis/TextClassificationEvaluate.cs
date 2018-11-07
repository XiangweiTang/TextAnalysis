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
            DataProcessing dp = new DataProcessing(Cfg);
            string textFilePath = dp.ProcessFile(Cfg.EvaluateDataPath);
            Digitalize dgt = new Digitalize(Cfg);
            string dataFilePath = dgt.Run(textFilePath);
            string resultPath = Common.RunScripts(Cfg, Cfg.EvaluateScriptPath, dataFilePath, Cfg.EvaluateLabelPath, Cfg.TextClassificationModelPath);

            File.Copy(resultPath, Cfg.EvaluateResultPath, true);
        }       

        public void RunBatches()
        {
            var batches = File.ReadLines(Cfg.UsedDataFile)
                .Where(x => x[0] != '#')
                .Select(x => x.Split('\t'))
                .Where(x => x[0] == Constants.SUP)
                .Select(x => x[1]);
            var resultList = batches.Select(x => RunSingleBatch(x)).ToList();
            foreach(string result in resultList)
                Console.WriteLine(result);
            File.WriteAllLines(Cfg.EvaluateResultPath, resultList);
        }

        public string RunSingleBatch(string batchName)
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
