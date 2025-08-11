using System;
using System.IO;
using System.Linq;

using SearchEngine;                 
using SearchEngine.UI;            
using SearchEngine.Core.Model;     

class Program
{
    static void Main(string[] args)
    {
        var defaultDocs = Path.Combine(AppContext.BaseDirectory, "docs");
        var docsPath = args.Length > 0 ? args[0] : defaultDocs;
        Directory.CreateDirectory(docsPath);

        Console.WriteLine($"[i] Indexing documents from: {Path.GetFullPath(docsPath)}");

        var engine = new SearchEngine.SearchEngine(docsPath);
        var ui = new ConsoleUi();

        Console.WriteLine("\nType your query (or type 'exit' to quit):");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            Console.ResetColor();

            var line = Console.ReadLine() ?? "";
            if (line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            var query = ParseQuery(line);
            var results = engine.Search(query).Take(10);

            ui.DisplayResults(results);
            Console.WriteLine();
        }
    }

    static SearchQuery ParseQuery(string line)
    {
        var mustInclude = new System.Collections.Generic.List<string>();
        var atLeastOne = new System.Collections.Generic.List<string>();
        var mustExclude = new System.Collections.Generic.List<string>();

        var index = line.IndexOf("get", StringComparison.Ordinal);
        if (index >= 0) line = line.Remove(index, 3);

        var matches = System.Text.RegularExpressions.Regex.Matches(line, @"[+-]?\"".*?\""|\S+")
                        .Cast<System.Text.RegularExpressions.Match>()
                        .Select(m => m.Value);

        foreach (var tok in matches)
        {
            if (tok.StartsWith("+"))
                atLeastOne.Add(tok.Substring(1).Trim('\"'));
            else if (tok.StartsWith("-"))
                mustExclude.Add(tok.Substring(1).Trim('\"'));
            else
                mustInclude.Add(tok.Trim('\"'));
        }

        return new SearchQuery(mustInclude, atLeastOne, mustExclude);
    }
}
