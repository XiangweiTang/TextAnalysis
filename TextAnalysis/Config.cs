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
        #region Args

        public string SimilarityInputPath { get; private set; } = @"D:\private\TextClassification_Old\Similar_input.txt";        
        public string ClfInputPath { get; private set; } = @"D:\private\TextClassification_Old\RawTest.txt";
        public string ClfOutputPath { get; private set; } = @"D:\private\TextClassification_Old\RawTest_result.txt";
        public string SimilarityOutputPath { get; private set; } = @"D:\public\tmp\simRes.txt";
        
        public string DemoFolder { get => Path.Combine(WorkFolder, "Demo"); }
        public string DemoSimStandardPath { get => Path.Combine(DemoFolder, "Data", "Python", "Similar.txt"); }
        public string DemoClfDictPath { get => Path.Combine(DemoFolder, "Data", "Python", "dict.txt"); }
        public string DemoPythonFolder { get => Path.Combine(DemoFolder, "Data", "Python"); }
        public string DemoClfModelPath { get => Path.Combine(DemoFolder, "Data", "Python", "PosNeg_Model.h5"); }
        public string TmpPath { get => Path.Combine(WorkFolder, "Tmp"); }

        public string InputFolder { get; private set; } = string.Empty;
        public string WorkFolder { get => @".\"; }
        public string PythonFolder { get { return Path.Combine(WorkFolder, "Data", "Python"); } }
        public string RawFolder { get { return Path.Combine(WorkFolder, "Data", "Raw"); } }
        public string PreProcessFolder { get { return Path.Combine(WorkFolder, "Data", "Pre"); } }
        public string WordBreakFolder { get { return Path.Combine(WorkFolder, "Data", "Wbr"); } }
        public string PostProcessFolder { get { return Path.Combine(WorkFolder, "Data", "Post"); } }
        public string TrainTextFolder { get { return Path.Combine(WorkFolder, "Data", "TrainText"); } }
        public string DevTextFolder { get { return Path.Combine(WorkFolder, "Data", "DevText"); } }
        public string TestTextFolder { get { return Path.Combine(WorkFolder, "Data", "TestText"); } }
        public string TrainFolder { get { return Path.Combine(WorkFolder, "Data", "Train"); } }
        public string DevFolder { get { return Path.Combine(WorkFolder, "Data", "Dev"); } }
        public string TestFolder { get { return Path.Combine(WorkFolder, "Data", "Test"); } }
        public string LogFolder { get { return Path.Combine(WorkFolder, "Log"); } }
        public string DictPath { get { return Path.Combine(WorkFolder, "Dict.txt"); } }
        public IEnumerable<Tuple<char,char>> KeptIntervals { get; private set; }
        public string DataLabel { get; private set; } = string.Empty;

        public int MaxVocab { get; private set; } = 10000;
        //public int PreserveOtherThanUnkPad { get; private set; } = 0;
        public double TestRatio { get; private set; } = 1;
        public double DevRatio { get; private set; } = 0;
        public int TestCount { get; private set; } = 1000;
        public int DevCount { get; private set; } = 1000;
        public bool UseCount { get; private set; } = false;
        public bool KeepSpace { get; private set; } = true;        
        //public bool UseExistingDict { get; private set; } = true;
        public bool ForceUpdate { get; private set; } = false;

        public string WordBreakType { get; private set; } = "NA";

        public string PythonPath { get; private set; } = @"C:\ProgramData\Anaconda3\python.exe";

        #endregion

        public Config()
        {
        }

        public void LoadConfig(string configPath)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configPath);
            ClfInputPath = xDoc["TextAnalysis"]["Demo"]["TextClassification"]["Input"].Attributes["Path"].Value;
            ClfOutputPath=xDoc["TextAnalysis"]["Demo"]["TextClassification"]["Output"].Attributes["Path"].Value;
            SimilarityInputPath= xDoc["TextAnalysis"]["Demo"]["Word2Vector"]["Input"].Attributes["Path"].Value;
            SimilarityOutputPath= xDoc["TextAnalysis"]["Demo"]["Word2Vector"]["Output"].Attributes["Path"].Value;
        }
    }
}
