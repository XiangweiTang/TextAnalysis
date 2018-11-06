using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class RunTextAnalysis
    {
        Config Cfg = new Config();
        public RunTextAnalysis(Config cfg)
        {
            Cfg = cfg;
            Init();
        }

        private void Init()
        {            
            Directory.CreateDirectory(Cfg.PythonScriptFolder);
            Directory.CreateDirectory(Cfg.NonSupTextFolder);
            Directory.CreateDirectory(Cfg.SupDataFolder);
            Directory.CreateDirectory(Cfg.SupLabelFolder);
            Directory.CreateDirectory(Cfg.SupTextFolder);

            CreatePythons(Constants.BUILD_MODEL);
            CreatePythons(Constants.DATA_PREPARE);
            CreatePythons(Constants.EVALUATE);
            CreatePythons(Constants.JIEBA);
            CreatePythons(Constants.PREDICT);
            CreatePythons(Constants.RESTORE_MODEL);
            CreatePythons(Constants.WORD2VEC);
        }

        private void CreatePythons(string name)
        {
            string internalPath = $"TextAnalysis.Python.{name}.py";
            string externalPath = Path.Combine(Cfg.PythonScriptFolder, name + ".py");
            var list = Common.ReadEmbed(internalPath);
            File.WriteAllLines(externalPath, list);
        }

        public void Run()
        {            
            AddNonSupData ansd = new AddNonSupData(Cfg);
            ansd.Run();

            AddSupData asd = new AddSupData(Cfg);
            asd.Run();

            TextClassificationTrain tct = new TextClassificationTrain(Cfg);
            tct.Run();

            TextClassificationEvaluate tce = new TextClassificationEvaluate(Cfg);
            tce.Run();

            TextClassificationPredict tcp = new TextClassificationPredict(Cfg);
            tcp.Run();

            Word2VecTrain wvt = new Word2VecTrain(Cfg);
            wvt.Run();

            Word2VecCalcSimilarity wvcs = new Word2VecCalcSimilarity(Cfg);
            wvcs.Run();
        }
    }
}
