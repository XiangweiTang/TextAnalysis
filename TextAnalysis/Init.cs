using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalysis
{
    class Init
    {
        Config Cfg = new Config();
        public Init(Config cfg)
        {
            Cfg = cfg;
            GlobalInit();
            DemoInit();
        }
        private void GlobalInit()
        {
            Directory.CreateDirectory(Cfg.PythonFolder);
            Directory.CreateDirectory(Cfg.TmpPath);
            PrepareFile(Cfg.PythonFolder, Constants.JiebaWbrInternalPath, Constants.JiebaWbrExternalPath);
            //TODO
        }

        private void DemoInit()
        {
            Directory.CreateDirectory(Cfg.DemoFolder);
            Directory.CreateDirectory(Path.Combine(Cfg.DemoFolder, "Data"));
            Directory.CreateDirectory(Path.Combine(Cfg.DemoFolder, "Data", "Python"));
            if (!File.Exists(Cfg.DemoClfModelPath))
            {
                var bin = Common.GetEmbed(Constants.PosNegModelInternalPath);
                File.WriteAllBytes(Cfg.DemoClfModelPath, bin);
            }
            PrepareFile(Cfg.DemoPythonFolder, Constants.RestoreModelInternalPath, Constants.RestoreModelExternalPath);
            PrepareFile(Cfg.DemoPythonFolder, Constants.DataPrepareInternalPath, Constants.DataPrepareExternalPath);
            PrepareFile(Cfg.DemoPythonFolder, Constants.PredictInternalPath, Constants.PredictExternalPath);
            PrepareFile(Cfg.DemoPythonFolder, Constants.ClfDictInternalPath, Constants.ClfDictExternalPath);
            PrepareFile(Cfg.DemoPythonFolder, Constants.SimilarInternalPath, Constants.SimilarExternalPath);
        }

        private void PrepareFile(string folderPath, string internalPath, string externalPath)
        {
            string filePath = Path.Combine(folderPath, externalPath);
            if (!File.Exists(filePath))
            {
                var list = Common.ReadEmbed(internalPath);
                File.WriteAllLines(filePath, list);
            }
        }
    }
}
