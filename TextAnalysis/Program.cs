using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Config cfg = new Config();
            Init(cfg);
            TextClassificationTrain tct = new TextClassificationTrain(cfg);
            tct.Run();
        }

        static void Init(Config cfg)
        {
            Directory.CreateDirectory(cfg.DataFolder);
            Directory.CreateDirectory(cfg.PythonScriptFolder);
            Directory.CreateDirectory(cfg.TmpFolder);
            Directory.CreateDirectory(cfg.SupDataFolder);
            Directory.CreateDirectory(cfg.SupLabelFolder);
            Directory.CreateDirectory(cfg.TextFolder);
            //Directory.CreateDirectory(cfg.NonlabeledFolder);
            Directory.CreateDirectory(cfg.NonSupDigitFolder);
            Directory.CreateDirectory(cfg.NonSupTextFolder);
            PreparePython("BuildModel.py", cfg.PythonScriptFolder);
            PreparePython("DataPrepare.py", cfg.PythonScriptFolder);
            PreparePython("Evaluate.py", cfg.PythonScriptFolder);
            PreparePython("Jieba.py", cfg.PythonScriptFolder);
            PreparePython("Predict.py", cfg.PythonScriptFolder);
            PreparePython("RestoreModel.py", cfg.PythonScriptFolder);
            PreparePython("Word2Vec.py", cfg.PythonScriptFolder);
        }

        static void PreparePython(string name, string pythonFolderPath)
        {
            string internalPath = "TextAnalysis.Python." + name;
            string externalPath = Path.Combine(pythonFolderPath, name);
            var list = Common.ReadEmbed(internalPath);
            if (!File.Exists(externalPath))
                File.WriteAllLines(externalPath, list);
        }
    }
}
