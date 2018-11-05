using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalysis
{
    class TrainTextClassification
    {
        Config Cfg = new Config();
        public TrainTextClassification(Config cfg)
        {
            Cfg = cfg;
        }

        public void Run()
        {
            PrepareTrainData ptd = new PrepareTrainData(Cfg);
            ptd.BuildTCData();
            Common.RunBuildTextClassification(ptd.TrainDataPath, ptd.TrainLabelPath, ptd.DevDataPath, ptd.DevLabelpath, Cfg.TextClassificationModelPath, Cfg.PythonPath, Cfg.BuildModelScriptPath);
        }
    }
}
