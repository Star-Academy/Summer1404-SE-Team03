namespace SearchEngine.Core.Interface
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string text);
    }
}