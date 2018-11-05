using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class TrainWord2Vec
    {
        Config Cfg = new Config();
        public TrainWord2Vec(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            var supList = Directory.EnumerateFiles(Cfg.SupTextFolder, $"*.{Cfg.Locale}.txt").SelectMany(x => File.ReadLines(x));
            var nonsupList = Directory.EnumerateFiles(Cfg.NonSupTextFolder, $"*.{Cfg.Locale}.txt").SelectMany(x => File.ReadLines(x));
            File.WriteAllLines(Cfg.Word2VecTrainDatapath, supList.Concat(nonsupList));
        }

        private void Train()
        {
            string args = string.Join(" ", Cfg.Word2VecScriptPath, Cfg.DictPath, Cfg.Word2VecTrainDatapath, Cfg.Word2VecKeyWordPath, Cfg.Word2VecSimilarityPath);
            Common.RunFile(Cfg.PythonPath, args);
        }
    }
}
