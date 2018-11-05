using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace TextAnalysis
{
    class NewDataProcessing
    {
        Config Cfg = new Config();
        public NewDataProcessing (Config cfg)
        {
            Cfg = cfg;
        }
        Regex TagReg = new Regex("<[^>]*>", RegexOptions.Compiled);
        Regex LinkReg = new Regex("http[^\\s]*", RegexOptions.Compiled);
        Regex SpaceReg = new Regex("\\s+", RegexOptions.Compiled);
        string[] intervals = new string[0];

        
        public void PostProcessFile(string inputFilePath, string outputFilePath)
        {
            var list = File.ReadLines(inputFilePath).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => TagReg.Replace(x, " ").Trim());
            File.WriteAllLines(outputFilePath, list);
        }
        public void WordBreakFile(string inputFilePath, string outputFilePath)
        {
            if (Cfg.Locale == "CHS")
                Common.RunWordBreak(inputFilePath, outputFilePath, Cfg.PythonPath, Cfg.JiebaScriptPath);
            else
                File.Copy(inputFilePath, outputFilePath);
        }
        public void PreProcessFile(string inputFilePath, string outputFilePath)
        {
            var list = File.ReadLines(inputFilePath).Select(x => RegCleanUp(x)).Select(x => CharCleanUp(x, intervals));
            File.WriteAllLines(outputFilePath, list);
        }
        private string RegCleanUp(string s)
        {
            string removeTag = TagReg.Replace(s.ToLower(), " ");
            string removeLink = LinkReg.Replace(removeTag, " ");
            string removeSpace = SpaceReg.Replace(removeLink, " ").Trim();
            return removeSpace;
        }
        private string CharCleanUp(string s,string[] intervals)
        {
            return new string(RemoveChars(s, intervals).ToArray());
        }
        private IEnumerable<char> RemoveChars(string s, string[] intervals)
        {
            foreach(char c in s)
            {
                foreach(string interval in intervals)
                {
                    if(interval[0]<=c&&c<=interval[1])
                    {
                        yield return c;
                        break;
                    }
                }
            }
        }
    }
}
