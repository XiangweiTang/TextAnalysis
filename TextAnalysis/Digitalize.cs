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
            if (Cfg.RebuildDict)
            {
                var fileList = Directory.EnumerateFiles(Cfg.NonSupTextFolder, $"*.{Cfg.Locale}.txt")
                    .Concat(Directory.EnumerateFiles(Cfg.SupTextFolder, $"*.{Cfg.Locale}.txt"));
                RebuildDictionary(fileList, Cfg.DictPath, Cfg.MaxVocab);                
            }
            Common.FolderTransport(Cfg.SupTextFolder, Cfg.SupDataFolder, TextToDigitFile);
        }

        private void RebuildDictionary(IEnumerable<string> fileList, string outputPath, int maxVocab)
        {
            var tail = fileList.SelectMany(x => x.Split(' ')).GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key);
            var list = Constants.HEAD.Concat(tail).Take(maxVocab).Select((x, y) => x + "\t" + y);
            File.WriteAllLines(outputPath, list);
        }

        public void TextToDigitFile(string wordPath, string dataPath)
        {
            var list = File.ReadLines(wordPath)
                .Select(x => x
                .Split(' ')
                .ArraySkip(256 - x.Length)
                .Select(y => WordToDigitDict.ContainsKey(y) ? WordToDigitDict[y] : WordToDigitDict[Constants.UNK]))
                .Select(x => string.Join(" ", x));
            File.WriteAllLines(dataPath, list);
        }
    }
}
