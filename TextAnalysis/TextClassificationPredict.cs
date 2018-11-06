using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class TextClassificationPredict
    {
        Config Cfg = new Config();
        string TmpName = string.Empty;
        public TextClassificationPredict(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            DataProcessing dataProcessing = new DataProcessing(Cfg);
            Digitalize digitalize = new Digitalize(Cfg);
            string prePath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.post");
            string dataPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.data");
            string resultPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.result");
            dataProcessing.PreProcessFile(Cfg.TextClassificationTestPath, prePath);
            dataProcessing.WordBreakFile(prePath, wbrPath);
            dataProcessing.PostProcessFile(wbrPath, postPath);
            digitalize.Run(postPath, dataPath);

            RunPredict(dataPath, resultPath);
            var list = File.ReadLines(resultPath).Zip(File.ReadLines(postPath), (x, y) => x + "\t" + y);
            File.WriteAllLines(Cfg.TextClassificationPredictResultPath, list);
        }

        private void RunPredict(string inputPath, string outputPath)
        {
            string args = string.Join(" ", Cfg.PredictScriptPath, inputPath, Cfg.TextClassificationModelPath, outputPath);
            Common.RunFile(Cfg.PythonPath, args);
        }
    }
}
