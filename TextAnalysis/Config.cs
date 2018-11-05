using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace TextAnalysis
{
    class Config
    {
        #region Input args
        public string WorkFolder { get; private set; } = ".\\";
        public string NonlabeledFolder { get; private set; } = string.Empty;
        public string NegativeFolder { get; private set; } = @"D:\public\tmp\Test\Neg";
        public string PositiveFolder { get; private set; } = @"D:\public\tmp\Test\Pos";
        public string BatchName { get; private set; } = "Check_20181031";
        public string[] ValidIntervals { get; private set; } = { "一龟", "  " };
        public string Locale { get; private set; } = "CHS";
        public string PythonPath { get; private set; } = @"C:\ProgramData\Anaconda3\python.exe";
        public double DevRate { get; private set; } = 0.1;
        public double TestRate { get; private set; } = 0.1;
        public bool AddLabeledData { get; private set; } = true;
        public bool AddNonLabeledData { get; private set; } = false;
        public bool RebuildDict { get; private set; } = true;
        public string DataDescription { get; private set; } = "This is only a test.";
        public int MaxVocab { get; private set; } = 10000;
        public string TextClassificationTestPath { get; private set; } = string.Empty;
        public string Word2VecTestPath { get; private set; } = string.Empty;
        #endregion

        #region Internal generated args
        public string PythonScriptFolder { get => Path.Combine(WorkFolder, "Python"); }
        public string JiebaScriptPath { get => Path.Combine(PythonScriptFolder, Constants.JIEBA + ".py"); }        
        public string BuildModelScriptPath { get => Path.Combine(PythonScriptFolder, Constants.BUILD_MODEL + ".py"); }
        public string EvaluateScriptPath { get => Path.Combine(PythonScriptFolder, Constants.EVALUATE + ".py"); }
        public string PredictScriptPath { get => Path.Combine(PythonScriptFolder, Constants.PREDICT + ".py"); }
        public string RestoreModelScriptPath { get => Path.Combine(PythonScriptFolder, Constants.RESTORE_MODEL + ".py"); }
        public string DataPrepareScriptPath { get => Path.Combine(PythonScriptFolder, Constants.DATA_PREPARE + ".py"); }
        public string Word2VecScriptPath { get => Path.Combine(PythonScriptFolder, Constants.WORD2VEC + ".py"); }

        public string UsedDataFile { get => Path.Combine(WorkFolder, "UsedData.txt"); }
        public string TmpFolder { get => Path.Combine(WorkFolder, "Tmp"); }
        public string DataFolder { get => Path.Combine(WorkFolder, "Data"); }        
        public string FileMappingPath { get => Path.Combine(DataFolder, "Mapping.txt"); }
        public string TextFolder { get => Path.Combine(DataFolder, "Text"); }
        public string NonSupTextFolder { get => Path.Combine(DataFolder, "NonSupText"); }
        public string NonSupDigitFolder { get => Path.Combine(DataFolder, "NonSupDigit"); }
        public string SupLabelFolder { get => Path.Combine(DataFolder, "SupLabel"); }
        public string SupTextFolder { get => Path.Combine(DataFolder, "SupText"); }
        public string SupDataFolder { get => Path.Combine(DataFolder, "SupData"); }
        public string DictPath { get => Path.Combine(DataFolder, $"{Locale}.dict"); }

        public string TextClassificationModelPath { get => Path.Combine(DataFolder, "TC.h5"); }
        public string TextClassificationTrainDataPath { get => Path.Combine(DataFolder, $"{Constants.TRAIN}.{Locale}.data"); }
        public string TextClassificationTrainLabelPath { get => Path.Combine(DataFolder, $"{Constants.TRAIN}.{Locale}.label"); }
        public string TextClassificationDevDataPath { get => Path.Combine(DataFolder, $"{Constants.DEV}.{Locale}.data"); }
        public string TextClassificationDevLabelPath { get => Path.Combine(DataFolder, $"{Constants.DEV}.{Locale}.label"); }
        public string TextClassificationResultPath { get => Path.Combine(WorkFolder, "Result.txt"); }

        public string Word2VecKeyWordPath { get => Path.Combine(WorkFolder, "Keywords.txt"); }
        public string Word2VecSimilarityPath { get => Path.Combine(DataFolder, "Similarity.txt"); }
        public string Word2VecTrainDatapath { get => Path.Combine(DataFolder, $"Total.text"); }
        #endregion

        public Config()
        {
            WorkFolder = Common.GetFullPath(WorkFolder);
        }
    }
}
