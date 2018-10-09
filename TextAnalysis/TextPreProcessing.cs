using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class TextPreProcessing
    {
        Config Cfg = new Config();
        public IEnumerable<Sample> DataList { get; private set; } = Enumerable.Empty<Sample>();
        public TextPreProcessing(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            var inputList = Common.GetSamples(Cfg.InputFolder);
            foreach (var sample in inputList)
            {
                string folderPath = Path.Combine(Cfg.PreProcessFolder, sample.Tag);
                Directory.CreateDirectory(folderPath);
                string fileName = sample.Content.Split('\\').Last();
                string outputFilePath = Path.Combine(folderPath, fileName);
                var list = File.ReadLines(sample.Content).Select(x => Cleanup(x));
                File.WriteAllLines(outputFilePath, list);
            }
        }

        private string Cleanup(string inputString)
        {
            return new string(CleanupChar(inputString).ToArray());
        }

        private IEnumerable<char> CleanupChar(string s)
        {
            foreach(char c in s)
            {
                if (Cfg.KeptIntervals.Any(x => x.Item1 <= c && c <= x.Item2))
                    yield return c;
                else if(Cfg.KeepSpace)
                    yield return ' ';
            }
        }
    }
}
