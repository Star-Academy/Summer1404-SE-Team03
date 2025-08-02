using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using SearchEngine.Core;

namespace SearchEngine.Core
{
    public class InvertedIndexManager
    {
        private readonly InvertedIndexData _invertedIndexData;
        private readonly ITokenizer _tokenizer;

        public InvertedIndexManager(ITokenizer tokenizer)
        {
            _invertedIndexData = new InvertedIndexData();
            _tokenizer = tokenizer;
        }

        public void AddDocument(string documentPath)
        {
            if (!FileReader.TryReadFile(documentPath, out var content))
            {
                return;
            }

            AddContentToIndex(content, documentPath);
        }

        public void AddTokenToIndex(string token, string documentIdentifier)
        {
            if (!_invertedIndexData.Index.TryGetValue(token, out var docSet))
            {
                docSet = new HashSet<string>();
                _invertedIndexData.Index[token] = docSet;
            }
            docSet.Add(documentIdentifier);
        }

        public IEnumerable<string> GetDocumentsForToken(string token)
        {
            if (_invertedIndexData.Index.TryGetValue(token, out var docSet))
            {
                return docSet;
            }
            return Enumerable.Empty<string>();
        }

        private void AddContentToIndex(string content, string documentIdentifier)
        {
            var tokens = _tokenizer.Tokenize(content);
            AddTokensToIndex(tokens, documentIdentifier);
        }

        private void AddTokensToIndex(IEnumerable<string> tokens, string documentIdentifier)
        {
            foreach (var token in tokens)
            {
                AddTokenToIndex(token, documentIdentifier);
            }
        }
        public IEnumerable<string> GetAllDocuments()
        {
            return _invertedIndexData.Index
                .SelectMany(kvp => kvp.Value)
                .Distinct();
        }
    }
}