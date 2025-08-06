using SearchEngine.Core.Processing;

namespace SearchEngine.Core.Interface
{
    public interface IInvertedIndex
    {
        public void AddTokenToIndex(string token, string documentIdentifier, INormalizer _normalizer);
        public IEnumerable<string> GetDocumentsForToken(string token);
        public IEnumerable<string> GetAllDocuments();
    }
}