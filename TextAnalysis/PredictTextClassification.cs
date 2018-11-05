using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class PredictTextClassification
    {
        Config Cfg = new Config();
        string TmpName = string.Empty;
        public PredictTextClassification(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
        }

        public void Run()
        {
            NewDataProcessing ndp = new NewDataProcessing(Cfg);
            TextToDigit t2d = new TextToDigit(Cfg);
            string prePath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.pre");
            string wbrPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.wbr");
            string postPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.post");
            string dataPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.data");
            string resultPath = Path.Combine(Cfg.TmpFolder, $"{TmpName}.{Cfg.Locale}.result");
            ndp.PreProcessFile(Cfg.TextClassificationTestPath, prePath);
            ndp.WordBreakFile(prePath, wbrPath);
            ndp.PostProcessFile(wbrPath, postPath);
            t2d.TextToDigitFile(postPath, dataPath);

            RunPredict(dataPath, resultPath);
            var list = File.ReadLines(resultPath).Zip(File.ReadLines(postPath), (x, y) => x + "\t" + y);
            File.WriteAllLines(Cfg.TextClassificationResultPath, list);
        }

        private void RunPredict(string inputPath, string outputPath)
        {
            string args = string.Join(" ", Cfg.PredictScriptPath, inputPath, Cfg.TextClassificationModelPath, outputPath);
            Common.RunFile(Cfg.PythonPath, args);
        }
    }
}
