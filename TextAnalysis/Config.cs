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
        public string Locale { get; private set; } = "CHS";
        public string NonlabeledFolder { get; private set; } = @"D:\private\DataSample\NonSup";
        public string NegativeFolder { get; private set; } = @"D:\public\tmp\Test\Neg";
        public string PositiveFolder { get; private set; } = @"D:\public\tmp\Test\Pos";
        public bool AddLabeledData { get; private set; } = false;
        public bool AddNonLabeledData { get; private set; } = false;
        public string BatchName { get; private set; } = "20181105_SUP";
        public string DataDescription { get; private set; } = "Supervised data, 20181105";

        public bool TrainTextClassification { get; private set; } = false;
        public bool TrainWord2Vec { get; private set; } = false;

        public bool RunPredict { get; private set; } = false;
        public string PredictTestPath { get; private set; } = @"C:\Users\v-xianta\Source\Repos\TextAnalysis\TextAnalysis\bin\Debug\Data\SupText\Check_20181031.Test.CHS.txt";
        public bool RunEvaluate { get; private set; } = false;
        public string EvaluatePosPath { get; private set; } = string.Empty;
        public string EvaluateNegPath { get; private set; } = string.Empty;
        public bool RunSimilarity { get; private set; } = false;
        public string SimilarityTestPath { get; private set; } = @"D:\private\DataSample\W2vTest\Sim_Test.txt";
        public string SimilarityKeyWordPath { get; private set; } = @"D:\private\Word2Vec\Test.txt";

        public string[] ValidIntervals { get; private set; } = { "一龟", "  " };

        public double DevRate { get; private set; } = 0.1;
        public double TestRate { get; private set; } = 0.1;
        public bool RebuildDict { get; private set; } = true;
        public int MaxVocab { get; private set; } = 50000;


        public string PythonPath { get; private set; } = @"C:\ProgramData\Anaconda3\python.exe";

        #endregion

        #region Internal generated args
        public string WorkFolder { get => Common.GetFullPath(".\\"); }
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
        public string NonSupTextFolder { get => Path.Combine(DataFolder, "NonSupText"); }
        public string SupLabelFolder { get => Path.Combine(DataFolder, "SupLabel"); }
        public string SupTextFolder { get => Path.Combine(DataFolder, "SupText"); }
        public string SupDataFolder { get => Path.Combine(DataFolder, "SupData"); }
        public string DictPath { get => Path.Combine(DataFolder, $"{Locale}.dict"); }

        public string TextClassificationModelPath { get => Path.Combine(DataFolder, $"{Locale}.h5"); }
        public string TextClassificationTrainDataPath { get => Path.Combine(DataFolder, $"{Constants.TRAIN}.{Locale}.data"); }
        public string TextClassificationTrainLabelPath { get => Path.Combine(DataFolder, $"{Constants.TRAIN}.{Locale}.label"); }
        public string TextClassificationDevDataPath { get => Path.Combine(DataFolder, $"{Constants.DEV}.{Locale}.data"); }
        public string TextClassificationDevLabelPath { get => Path.Combine(DataFolder, $"{Constants.DEV}.{Locale}.label"); }
        public string TextClassificationEvaluationResultPath { get => Path.Combine(WorkFolder, "TextClassificationEvaluationResult.txt"); }
        public string TextClassificationPredictResultPath { get => Path.Combine(WorkFolder, "TextClassificationPredictResult.txt"); }

        public string Word2VecSimilarityPath { get => Path.Combine(DataFolder, $"Similarity.{Locale}.txt"); }
        public string Word2VecTrainDatapath { get => Path.Combine(DataFolder, $"Total.text"); }
        public string Word2VecResultDetailPath { get => Path.Combine(WorkFolder, "Word2VecDetailResult.txt"); }
        public string Word2VecResultBriefPath { get => Path.Combine(WorkFolder, "Word2VecBriefResult.txt"); }
        #endregion

        public Config()
        {
            
        }

        public void LoadConfig(string configPath)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configPath);

            Locale = xDoc.GetXmlValue("TextAnalysis", "Locale").ToUpper();
            PythonPath = xDoc.GetXmlValue("TextAnalysis/Python", "Path");

            #region Data
            BatchName = xDoc.GetXmlValue("TextAnalysis/NewData", "DataBatchName");
            DataDescription = xDoc.GetXmlValue("TextAnalysis/NewData", "DataDescription");

            AddNonLabeledData = xDoc.GetXmlValueBool("TextAnalysis/NewData/NonLabeledData", "Add");
            NonlabeledFolder = xDoc.GetXmlValue("TextAnalysis/NewData/NonLabeledData", "Path");
            AddLabeledData= xDoc.GetXmlValueBool("TextAnalysis/NewData/LabeledData", "Add");
            PositiveFolder = xDoc.GetXmlValue("TextAnalysis/NewData/LabeledData", "PosPath");
            NegativeFolder = xDoc.GetXmlValue("TextAnalysis/NewData/LabeledData", "NegPath");
            #endregion

            #region Train
            TrainTextClassification = xDoc.GetXmlValueBool("TextAnalysis/Train", "TextClassification");
            TrainWord2Vec = xDoc.GetXmlValueBool("TextAnalysis/Train", "Word2Vec");
            #endregion

            #region Test
            RunEvaluate = xDoc.GetXmlValueBool("TextAnalysis/Test/Evaluate", "Run");
            EvaluatePosPath = xDoc.GetXmlValue("TextAnalysis/Test/Evaluate", "PosPath");
            EvaluateNegPath = xDoc.GetXmlValue("TextAnalysis/Test/Evaluate", "NegPath");
            RunPredict = xDoc.GetXmlValueBool("TextAnalysis/Test/Predict", "Run");
            PredictTestPath = xDoc.GetXmlValue("TextAnalysis/Test/Predict", "TestFilePath");
            RunSimilarity = xDoc.GetXmlValueBool("TextAnalysis/Test/Similarity", "Run");
            SimilarityTestPath = xDoc.GetXmlValue("TextAnalysis/Test/Similarity", "TestFilePath");
            SimilarityKeyWordPath=xDoc.GetXmlValue("TextAnalysis/Test/Similarity", "KeywordPath");
            #endregion

            #region Intevals
            var intervalDict = xDoc.SelectNodes("TextAnalysis/ValidIntervals/Interval")
                .Cast<XmlNode>()
                .GroupBy(x => x.Attributes["Locale"].Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Attributes["Value"].Value));
            ValidIntervals = intervalDict[Locale].ToArray();
            #endregion

            #region Model
            TestRate = xDoc.GetXmlValueDouble("TextAnalysis/Model", "TestRate");
            DevRate=xDoc.GetXmlValueDouble("TextAnalysis/Model", "DevRate");
            MaxVocab = xDoc.GetXmlValueInt("TextAnalysis/Model", "MaxVocab");
            RebuildDict = xDoc.GetXmlValueBool("TextAnalysis/Model", "RebuildDict");
            #endregion
        }
    }
}
