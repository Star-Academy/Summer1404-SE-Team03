using System.Text.RegularExpressions;
using SearchEngine.Core.Model;

namespace SearchEngine.UI
{
    public class ConsoleUi
    {
        public SearchQuery GetQueryFromUser()
        {
            Console.Write("Enter query: ");
            var line = Console.ReadLine() ?? "";
            var index = line.IndexOf("get");
            line = (index < 0) ? line : line.Remove(index, 3);
            
                
            var mustInclude = new List<string>();
            var atLeastOne = new List<string>();
            var mustExclude = new List<string>();

            var tokens = Regex.Matches(line, @"[+-]?\"".*?\""|\S+")
                .Cast<Match>()
                .Select(m => m.Value);

            foreach (var tok in tokens)
            {
                if (tok.StartsWith("+"))
                {
                    atLeastOne.Add(tok.Substring(1).Trim('\"'));
                }
                else if (tok.StartsWith("-"))
                {
                    mustExclude.Add(tok.Substring(1).Trim('\"'));
                }
                else
                {
                    mustInclude.Add(tok.Trim('\"'));
                }
            }

            return new SearchQuery(mustInclude, atLeastOne, mustExclude);
        }

        public void DisplayResults(IEnumerable<string> results)
        {
            Console.WriteLine("\nSearch results:");
            if (!results.Any())
            {
                Console.WriteLine("No documents found.");
                return;
            }
            foreach (var doc in results)
                Console.WriteLine(Path.GetFileName(doc));
        }
    }
}