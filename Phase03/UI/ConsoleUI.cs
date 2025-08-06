using SearchEngine.Core.Model;
using SearchEngine.Core.Interface;
using SearchEngine.Core.Processing;

namespace SearchEngine.UI
{
    public class ConsoleUi
    {
        public T GetQueryFromUser<T>(INormalizer _normalizer) where T : ISearchQuery, new()
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

            T query = new T();
            query.MustInclude = _normalizer.Normalize(mustInclude);
            query.AtLeastOne = _normalizer.Normalize(atLeastOne);
            query.MustExclude = _normalizer.Normalize(mustExclude);
            return query;
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