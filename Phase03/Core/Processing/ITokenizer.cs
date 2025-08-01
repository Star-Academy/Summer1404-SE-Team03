namespace SearchEngine.Core.Processing
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string text);
    }
}