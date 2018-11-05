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

            TrainTextClassification ttc = new TrainTextClassification(Cfg);
            ttc.Run();

            EvaluateTextClassification etc = new EvaluateTextClassification(Cfg);
            etc.Run();

            PredictTextClassification ptc = new PredictTextClassification(Cfg);
            ptc.Run();

            TrainWord2Vec twv = new TrainWord2Vec(Cfg);
            twv.Run();

            CalcSimilarity cs = new CalcSimilarity(Cfg);
            cs.Run();
        }
    }
}
