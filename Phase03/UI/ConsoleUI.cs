using SearchEngine.Core.Model;

namespace SearchEngine.UI
{
    public class ConsoleUi
    {
        public SearchQuery GetQueryFromUser()
        {
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