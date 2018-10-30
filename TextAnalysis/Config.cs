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
        public string WorkFolder { get; private set; } = string.Empty;
        public string NegativeFolder { get; private set; } = string.Empty;
        public string PositiveFolder { get; private set; } = string.Empty;
        public string BatchName { get; private set; } = string.Empty;
        public string[] ValidIntervals { get; private set; } = new string[0];
        public string Locale { get; private set; } = string.Empty;
        public string PythonPath { get; private set; } = string.Empty;
        public double DevRate { get; private set; } = 0.1;
        public double TestRate { get; private set; } = 0.1;
        #endregion

        #region Internal generated args
        public string PythonScriptFolder { get => Path.Combine(WorkFolder, "Python"); }
        public string JiebaScriptPath { get => Path.Combine(PythonScriptFolder, "Jieba.py"); }
        public string DataFolder { get => Path.Combine(WorkFolder, "Data"); }        
        public string FileMappingPath { get => Path.Combine(DataFolder, "Mapping.txt"); }
        public string TextFolder { get => Path.Combine(DataFolder, "Text"); }
        public string NonLabelTextFolder { get => Path.Combine(DataFolder, "NonLabel"); }
        public string LabelFolder { get => Path.Combine(DataFolder, "Label"); }
        public string DigitFolder { get => Path.Combine(DataFolder, "Digit"); }
        public string UsedDataFile { get => Path.Combine(DataFolder, "UsedData.txt"); }
        public string TmpFolder { get => Path.Combine(WorkFolder, "Tmp"); }        
        #endregion
    }
}
