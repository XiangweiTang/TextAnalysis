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
        public static readonly string[] Kept = { PAD, UNK, };
        public const string DEV = "Dev";
        public const string TEST = "Test";
        public const string TRAIN = "Train";
    }
}
