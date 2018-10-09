using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Config
    {
        public string InputFolder { get; private set; } = @"D:\public\tmp\Input";
        public string WorkFolder { get; private set; } = @"D:\public\tmp";
        public string PythonFolder { get { return Path.Combine(WorkFolder, "Python"); } }
        public string PreProcessFolder { get { return Path.Combine(WorkFolder, "Data", "Pre"); } }
        public string WbrFolder { get { return Path.Combine(WorkFolder, "Data", "Wbr"); } }
        public string PostProcessFolder { get { return Path.Combine(WorkFolder, "Data", "Post"); } }
        public IEnumerable<Tuple<char,char>> KeptIntervals { get; private set; }

        public int MaxVocab { get; private set; } = 10000;
        public int PreserveOtherThanUnkPad { get; private set; } = 0;
        public double TestRatio { get; private set; } = 1;
        public double DevRatio { get; private set; } = 0;
        public int TestCount { get; private set; } = 1000;
        public int DevCount { get; private set; } = 1000;
        public bool UseCount { get; private set; } = false;
        public bool KeepSpace { get; private set; } = true;
        public bool RunWordBreak { get; private set; } = false;

        public string PythonPath { get; private set; } = string.Empty;

        public Config() { KeptIntervals = new List<Tuple<char, char>>() {new Tuple<char, char>( '一','龟')};       }
    }
}
