using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace TextAnalysis
{
    class TextToDigit
    {        
        private Dictionary<string, string> TextToDigitDict = new Dictionary<string, string>();     
        private Sample[] TrainTextList = new Sample[0];        
        private Sample[] TestTextList = new Sample[0];        
        private Sample[] DevTextList = new Sample[0];
        private string Replace = " ";

        private Config Cfg = new Config();
        private int TestCount = 0;
        private int DevCount = 0;

        public TextToDigit(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            if (!Cfg.KeepSpace)
                Replace = string.Empty;
            var inputList = Common.GetSamples(Cfg.WbrFolder);
            var dataArray = inputList.SelectMany(x => ProcessSingleFile(x)).Shuffle();
            SetCount(dataArray.Length);
            TestTextList = dataArray.ArrayTake(TestCount);
            DevTextList = dataArray.ArraySkip(TestCount).ArrayTake(DevCount);
            TrainTextList = dataArray.ArraySkip(TestCount + DevCount);
            BuildDict();
            Output();
        }

        private void SetCount(int total)
        {
            if (Cfg.UseCount)
            {
                TestCount = Math.Max(Cfg.TestCount, 0);
                DevCount = Math.Max(Cfg.DevCount, 0);                
            }
            else
            {
                TestCount = Math.Max(Convert.ToInt32(total * Cfg.TestRatio), Cfg.TestCount > 0 ? 1 : 0);
                DevCount = Math.Max(Convert.ToInt32(total * Cfg.DevRatio), Cfg.DevCount > 0 ? 1 : 0);
            }
        }

        private IEnumerable<Sample> ProcessSingleFile(Sample s)
        {
            string tag = s.Tag;
            string path = s.Content;
            char[] sep = { ' ' };
            return File.ReadLines(path)
                .Select(x =>new Sample { Tag = tag, Content = Regex.Replace(x, "\\s+", Replace).Trim() });
        }

        //private void BuildDict()
        //{
        //    int startIndex = 2 + Cfg.PreserveOtherThanUnkPad;
        //    TextToDigitDict = TrainTextList
        //        .SelectMany(x => x.Content.Split(' '))
        //        .GroupBy(x => x)
        //        .OrderByDescending(x => x.Count())
        //        .Take(Cfg.MaxVocab)
        //        .ToDictionary(x => x.Key, x => startIndex++);
        //    TextToDigitDict.Add("<UNK>", 0);
        //    TextToDigitDict.Add("<PAD>", 0);
        //}

            private void BuildDict()
        {
            int startIndex = 2 + Cfg.PreserveOtherThanUnkPad;
            TextToDigitDict = File.ReadAllLines(@"D:\private\TextClassification\dict.txt")
                .ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
        }

        private Sample Text2Digit(Sample s)
        {
            string newContent = s.Content
                .Split(' ')
                .Select(x => TextToDigitDict.ContainsKey(x) ? TextToDigitDict[x].ToString() : "0")
                .Aggregate((x, y) => x + " " + y);
            return new Sample { Tag = s.Tag, Content = newContent };
        }

        private void Output()
        {
            Output(TrainTextList, "Train");
            Output(TestTextList, "Test");
            Output(DevTextList, "Dev");

            
        }

        private void Output(Sample[] textList, string name)
        {
            var tags = textList.Select(x => x.Tag);
            string tagPath = Path.Combine(Cfg.PostProcessFolder, name + "_label.txt");
            File.WriteAllLines(tagPath, tags);

            var contents = textList.Select(x => x.Content);
            string textPath = Path.Combine(Cfg.PostProcessFolder, name + "_text.txt");
            File.WriteAllLines(textPath, contents);

            var digits = textList.Select(x => Text2Digit(x).Content);
            string digitPath = Path.Combine(Cfg.PostProcessFolder, name + "_data.txt");
            File.WriteAllLines(digitPath, digits);
        }

        private void OutputDict()
        {
            string t2dPath = Path.Combine(Cfg.PostProcessFolder, "TextToDigit.dict");
            File.WriteAllLines(t2dPath, TextToDigitDict.Select(x => x.Key + "\t" + x.Value));

            string d2tPath = Path.Combine(Cfg.PostProcessFolder, "DigitToText.dict");
            File.WriteAllLines(d2tPath, TextToDigitDict.Select(x => x.Value + "\t" + x.Key));
        }
    }
}
