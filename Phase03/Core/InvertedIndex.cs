using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using SearchEngine.Core;
using SearchEngine.Core.Interface;

namespace SearchEngine.Core
{
    public class InvertedIndexManager : IInvertedIndex
    {
        private IInvertedIndexData _invertedIndexData;

        public InvertedIndexManager(IInvertedIndexData data)
        {
            _invertedIndexData = data;
        }

        public void Update(IInvertedIndexData data)
        {
            _invertedIndexData = data;
        }

        public void AddDocument(string documentPath, ITokenizer _tokenizer, INormalizer _nomalizer)
        {
            if (!FileReader.TryReadFile(documentPath, out var content))
            {
                return;
            }

            content = _nomalizer.Normalize(content);
            AddContentToIndex(content, documentPath, _tokenizer);
        }

        private void AddTokenToIndex(string token, string documentIdentifier)
        {
            if (!_invertedIndexData.Index.TryGetValue(token, out var docSet))
            {
                docSet = new HashSet<string>();
                _invertedIndexData.Index[token] = docSet;
            }
            docSet.Add(documentIdentifier);
        }

        public void AddTokenToIndex(string token, string documentIdentifier, INormalizer _nomalizer)
        {
            token = _nomalizer.Normalize(token);
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

        private void AddContentToIndex(string content, string documentIdentifier, ITokenizer _tokenizer)
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