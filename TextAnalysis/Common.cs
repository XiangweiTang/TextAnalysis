﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace TextAnalysis
{
    internal static class Common
    {
        private static Random R = new Random();
        public static T[] Shuffle<T>(this IEnumerable<T> collection)
        {
            T[] array = collection.ToArray();
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int j = R.Next(n);
                T t = array[i];
                array[i] = array[j];
                array[j] = t;
            }
            return array;
        }

        public static T[] ArraySkip<T>(this T[] inputArray, int skip)
        {
            int inputLength = inputArray.Length;
            if (skip < 0)
                skip = 0;
            skip = Math.Min(skip, inputLength);
            T[] outputArray = new T[inputLength - skip];
            Array.Copy(inputArray, skip, outputArray, 0, inputLength - skip);
            return outputArray;
        }

        public static T[] ArrayTake<T>(this T[] inputArray, int take)
        {
            int inputLength = inputArray.Length;
            if (take < 0)
                take = 0;
            take = Math.Min(take, inputLength);
            T[] outputArray = new T[take];
            Array.Copy(inputArray, outputArray, take);
            return outputArray;
        }

        public static T[] ArrayRange<T>(this T[] inputArrary, int index, int count)
        {
            int inputLength = inputArrary.Length;
            if (index < 0)
                index = 0;
            index = Math.Min(index, inputLength);
            if (count < 0)
                count = 0;
            count = Math.Min(count, inputLength - index);
            T[] outputArray = new T[count];
            Array.Copy(inputArrary, index, outputArray, 0, count);
            return outputArray;
        }

        public static void RunFile(string filePath, string args, string workDir = "", bool newWindow = false)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = filePath;
            proc.StartInfo.Arguments = args;
            if (!string.IsNullOrWhiteSpace(workDir))
                proc.StartInfo.WorkingDirectory = workDir;
            proc.StartInfo.UseShellExecute = newWindow;

            proc.Start();
            proc.WaitForExit();
        }

        public delegate void ProcessFile(string inputFilePath, string outputFilePath);
        public static void FolderTransport(string inputFolder, string outputFolder, ProcessFile pf)
        {
            foreach (string inputFilePath in Directory.EnumerateFiles(inputFolder))
            {
                Console.WriteLine("Processing " + inputFilePath);
                string fileName = inputFilePath.Split('\\').Last();
                string outputFilePath = Path.Combine(outputFolder, fileName);
                pf(inputFilePath, outputFilePath);
            }
        }

        public static void RunWordBreak(string inputFilePath, string outputFilePath, string pythonPath, string scriptPath)
        {
            string args = string.Join(" ", scriptPath, inputFilePath, outputFilePath);
            RunFile(pythonPath, args);
        }
        public static IEnumerable<string> ReadEmbed(string path)
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(asmb.GetManifestResourceStream(path)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    yield return line;
            }
        }

        public static string GetFullPath(string relativePath)
        {
            return new DirectoryInfo(relativePath).FullName;
        }

        public static string GetXmlValue(this XmlNode node, string xpath, string attribute)
        {
            XmlNode targetNode = node.SelectSingleNode(xpath);
            if (string.IsNullOrWhiteSpace(attribute))
                return targetNode.InnerText;
            else
                return targetNode.Attributes[attribute].Value;
        }

        public static bool GetXmlValueBool(this XmlNode node, string xpath, string attribute)
        {
            return bool.Parse(GetXmlValue(node, xpath, attribute));
        }

        public static double GetXmlValueDouble(this XmlNode node, string xpath, string attribute)
        {
            return double.Parse(GetXmlValue(node, xpath, attribute));
        }

        public static int GetXmlValueInt(this XmlNode node, string xpath, string attribute)
        {
            return int.Parse(GetXmlValue(node, xpath, attribute));
        }

        public static string RunScripts(Config cfg, string scriptPath, params string[] args)
        {
            string outputPath = cfg.TmpFilePath();
            string argString = scriptPath + " " + string.Join(" ", args) + " " + outputPath;
            RunFile(cfg.PythonPath, argString);
            return outputPath;
        }

        public static string TmpFilePath(this Config cfg)
        {
            string tmpName = Guid.NewGuid().ToString();
            return Path.Combine(cfg.TmpFolder, tmpName + ".txt");
        }
    }
}
