using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class PrepareTrainData
    {
        Config Cfg = new Config();
        public string TrainDataPath { get; private set; } = string.Empty;
        public string TrainLabelPath { get; private set; } = string.Empty;
        public string DevDataPath { get; private set; } = string.Empty;
        public string DevLabelpath { get; private set; } = string.Empty;
        private string TmpName = string.Empty;
        public PrepareTrainData(Config cfg)
        {
            Cfg = cfg;
            TmpName = Guid.NewGuid().ToString();
            DataClean dc = new DataClean(Cfg);
            dc.RunAddTrainData();
        }
        public void BuildW2VData()
        {
            var usedList = GetUsedData(Cfg.NonSupTextFolder).ToArray();
            TrainDataPath = Path.Combine(Cfg.TmpFolder, TmpName + "_train_nonsup_data.txt");
            RebuildFiles(usedList, TrainDataPath, Cfg.NonSupTextFolder, "{0}.txt");
        }

        public void BuildTCData()
        {
            var usedList = GetUsedData(Cfg.SupLabelFolder).ToArray();

            TrainDataPath = Path.Combine(Cfg.TmpFolder, TmpName + "_train_sup_data.txt");
            RebuildFiles(usedList, TrainDataPath, Cfg.SupDigitFolder, "{0}_train.txt");

            TrainLabelPath = Path.Combine(Cfg.TmpFolder, TmpName + "_train_sup_label.txt");
            RebuildFiles(usedList, TrainLabelPath, Cfg.SupLabelFolder, "{0}_train.txt");

            DevDataPath = Path.Combine(Cfg.TmpFolder, TmpName + "_dev_sup_data.txt");
            RebuildFiles(usedList, DevDataPath, Cfg.SupDigitFolder, "{0}_dev.txt");

            DevLabelpath = Path.Combine(Cfg.TmpFolder, TmpName + "_dev_sup_label.txt");
            RebuildFiles(usedList, DevLabelpath, Cfg.SupLabelFolder, "{0}_dev.txt");
        }        

        private void RebuildFiles(string[] usedList, string filePath, string folderName, string template)
        {
            var list = usedList.SelectMany(x => File.ReadLines(Path.Combine(folderName, string.Format(template, x))));                
            File.WriteAllLines(filePath, list);
        }

        private IEnumerable<string> GetUsedData(string dataFolder)
        {
            foreach (string usedData in File.ReadLines(Cfg.UsedDataFile))
            {
                if (usedData[0] == '#')
                    continue;
                string locale = usedData.Split('\t')[0];
                if (locale != Cfg.Locale)
                    continue;
                string batchName = usedData.Split('\t')[1];
                string dataPath = Path.Combine(dataFolder, $"{batchName}_{Constants.TRAIN}.txt");                
                if (!File.Exists(dataPath))
                    Console.WriteLine($"File doesn't exist:\t{dataPath}");
                else
                    yield return batchName;
            }
        }
    }
}
