using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysis
{
    internal static class Constants
    {
        public const string UNK = "<UNK>";
        public const string PAD = "<PAD>";        
        public static readonly string[] HEAD = { PAD, UNK, };
        public const string SUP = "SUP";
        public const string NONSUP = "NONSUP";
        public const string DEV = "Dev";
        public const string TEST = "Test";
        public const string TRAIN = "Train";

        public const string BUILD_MODEL = "BuildModel";
        public const string DATA_PREPARE = "DataPrepare";
        public const string EVALUATE = "Evaluate";
        public const string JIEBA = "Jieba";
        public const string PREDICT = "Predict";
        public const string RESTORE_MODEL = "RestoreModel";
        public const string WORD2VEC = "Word2Vec";
    }
}
