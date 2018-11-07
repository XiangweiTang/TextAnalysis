using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class TextClassificationTrain
    {
        Config Cfg = new Config();
        public TextClassificationTrain(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            PrepareData(Constants.DEV);
            PrepareData(Constants.TRAIN);
            Train();
            TextClassificationEvaluate eva = new TextClassificationEvaluate(Cfg);
            eva.RunBatches();
        }

        private void PrepareData(string type)
        {
            var textList = Directory.EnumerateFiles(Cfg.SupDataFolder, $"*.{type}.{Cfg.Locale}.txt")
                .SelectMany(x => File.ReadLines(x));
            var labelList = Directory.EnumerateFiles(Cfg.SupLabelFolder, $"*.{type}.{Cfg.Locale}.txt")
                .SelectMany(x => File.ReadLines(x));
            if (type == Constants.TRAIN)
            {
                File.WriteAllLines(Cfg.TextClassificationTrainDataPath, textList);
                File.WriteAllLines(Cfg.TextClassificationTrainLabelPath, labelList);
            }
            if (type == Constants.DEV)
            {
                File.WriteAllLines(Cfg.TextClassificationDevDataPath, textList);
                File.WriteAllLines(Cfg.TextClassificationDevLabelPath, labelList);
            }            
        }

        private void Train()
        {
            string args = string.Join(" ", Cfg.BuildModelScriptPath, Cfg.TextClassificationTrainDataPath, Cfg.TextClassificationTrainLabelPath, Cfg.TextClassificationDevDataPath, Cfg.TextClassificationDevLabelPath, Cfg.TextClassificationModelPath);
            Common.RunFile(Cfg.PythonPath, args);
        }
    }
}
