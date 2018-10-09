using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace TextAnalysis
{
    internal static class Common
    {
        private static Random R = new Random();
        public static T[] Shuffle<T>(this IEnumerable<T> collection)
        {
            T[] array = collection.ToArray();
            int n = array.Length;
            for(int i = 0; i < n; i++)
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
            skip = Math.Min(skip, inputLength);
            T[] outputArray = new T[inputLength - skip];
            Array.Copy(inputArray, skip, outputArray, 0, inputLength - skip);
            return outputArray;
        }

        public static T[] ArrayTake<T>(this T[] inputArray, int take)
        {
            int inputLength = inputArray.Length;
            take = Math.Min(take, inputLength);
            T[] outputArray = new T[take];
            Array.Copy(inputArray, outputArray, take);
            return outputArray;
        }

        public static IEnumerable<Sample> GetSamples(string folderPath)
        {
            foreach(string path in Directory.EnumerateDirectories(folderPath))
            {
                string tag = path.Split('\\').Last();
                foreach (string filePath in Directory.EnumerateFiles(path))
                    yield return new Sample { Tag = tag, Content = filePath };
            }
        }

        public static void RunFile(string filePath, string args, string workDir="", bool newWindow = false)
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
    }
}
