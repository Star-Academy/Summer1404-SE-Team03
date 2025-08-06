namespace SearchEngine.Core.Interface
{
    public interface INormalizer
    {
        public string Normalize(string text);
        public IEnumerable<string> Normalize(IEnumerable<string> data);
    }
}