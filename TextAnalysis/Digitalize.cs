using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Digitalize
    {
        Dictionary<string, string> WordToDigitDict = new Dictionary<string, string>();
        Config Cfg = new Config();
        public Digitalize(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            Prepare();
            Common.FolderTransport(Cfg.SupTextFolder, Cfg.SupDataFolder, TextToDigit);
        }

        public void Run(string wordPath, string dataPath)
        {
            Prepare();
            TextToDigit(wordPath, dataPath);
        }

        private void Prepare()
        {
            if (Cfg.RebuildDict)
            {
                var fileList = Directory.EnumerateFiles(Cfg.NonSupTextFolder, $"*.{Cfg.Locale}.txt")
                    .Concat(Directory.EnumerateFiles(Cfg.SupTextFolder, $"*.{Cfg.Locale}.txt"));
                RebuildDictionary(fileList, Cfg.DictPath, Cfg.MaxVocab);
            }
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            WordToDigitDict = File.ReadLines(Cfg.DictPath).ToDictionary(x => x.Split('\t')[0], x => x.Split('\t')[1]);
        }

        private void RebuildDictionary(IEnumerable<string> fileList, string outputPath, int maxVocab)
        {
            var tail = fileList.SelectMany(x=>File.ReadLines(x)).SelectMany(x => x.Split(' ')).GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key);
            var list = Constants.HEAD.Concat(tail).Take(maxVocab).Select((x, y) => x + "\t" + y);
            File.WriteAllLines(outputPath, list);
        }

        private void TextToDigit(string wordPath, string dataPath)
        {
            var list = File.ReadLines(wordPath)
                .Select(x => x
                .Split(' '))
                .Select(x=>x
                .ArraySkip(x.Length - 256)
                .Select(y => WordToDigitDict.ContainsKey(y) ? WordToDigitDict[y] : WordToDigitDict[Constants.UNK]))
                .Select(x => string.Join(" ", x));
            File.WriteAllLines(dataPath, list);
        }

        private bool DoNothing(string[] list)
        {
            if (list.Length >= 256)
                ;
            return true;
        }
    }
}
