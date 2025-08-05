namespace SearchEngine.Core.Processing
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string text);
        IEnumerable<(string Token, int Position)> TokenizeWithPositions(string content);
    }
}