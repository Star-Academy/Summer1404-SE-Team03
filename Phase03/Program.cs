using System;
using System.Collections.Generic;
using System.IO;
using Phase02.Core;

namespace Phase02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var baseDir = AppContext.BaseDirectory;
            // baseDir = ...\bin\Debug\net8.0\
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var dataDir = Path.Combine(projectRoot, "EnglishData");
            var files = FileReader.ReadAllFileNames(dataDir);
            var index = new InvertedIndex();

            foreach (var file in files)
                index.AddDocument(file);

            Console.Write("Enter query: ");
            var line = Console.ReadLine() ?? "";

            var mustInclude = new List<string>();
            var atLeastOne = new List<string>();
            var mustExclude = new List<string>();

            foreach (var tok in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (tok.StartsWith("+"))
                    atLeastOne.Add(tok.Substring(1));
                else if (tok.StartsWith("-"))
                    mustExclude.Add(tok.Substring(1));
                else
                    mustInclude.Add(tok);
            }

            var results = index.SmartSearch(mustInclude, atLeastOne, mustExclude);

            Console.WriteLine("\nSearch results:");
            foreach (var doc in results)
                Console.WriteLine(Path.GetFileName(doc));
        }
    }
}
