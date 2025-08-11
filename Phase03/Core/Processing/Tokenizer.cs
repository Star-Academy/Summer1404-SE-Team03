namespace SearchEngine.Core.Processing
{
    public class Tokenizer : ITokenizer
    {
        private readonly INormalizer _normalizer;

        public Tokenizer(INormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public IEnumerable<string> Tokenize(string text)
        {
            var normalizedText = _normalizer.Normalize(text);
            return normalizedText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}