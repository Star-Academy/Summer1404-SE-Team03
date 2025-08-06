using SearchEngine.Core.Interface;

namespace SearchEngine.Core.Processing
{
    public class Tokenizer : ITokenizer
    {
        public IEnumerable<string> Tokenize(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}