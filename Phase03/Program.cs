using System.Diagnostics.CodeAnalysis;
using SearchEngine.Core;
using SearchEngine.UI;

namespace SearchEngine
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var dataDir = AppConfig.DataDirectory;
            var searchEngine = new SearchEngine(dataDir);

            var consoleUi = new ConsoleUi();
            var query = consoleUi.GetQueryFromUser();
            var results = searchEngine.Search(query);
            consoleUi.DisplayResults(results);
        }
    }
}