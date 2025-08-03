using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;
using System.Collections.Generic;
using System.Linq;

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

        public void AddTokenToIndex(string token, string documentIdentifier, int position)
        {
            if (!_invertedIndexData.Index.TryGetValue(token, out var postingsList))
            {
                postingsList = new Dictionary<string, HashSet<int>>();
                _invertedIndexData.Index[token] = postingsList;
            }

            if (!postingsList.TryGetValue(documentIdentifier, out var positions))
            {
                positions = new HashSet<int>();
                postingsList[documentIdentifier] = positions;
            }

            positions.Add(position);
        }

        public IEnumerable<string> GetDocumentsForToken(string tokenOrPhrase)
        {
            tokenOrPhrase = tokenOrPhrase.ToUpper();
            var words = tokenOrPhrase.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                if (_invertedIndexData.Index.TryGetValue(words[0], out var postingsList))
                {
                    return postingsList.Keys;
                }
                return Enumerable.Empty<string>();
            }
            
            if (words.Length == 0 || !_invertedIndexData.Index.TryGetValue(words[0], out var firstWordPostings))
            {
                return Enumerable.Empty<string>();
            }

            var matchingDocs = new List<string>(firstWordPostings.Keys);

            for (int i = 1; i < words.Length; i++)
            {
                var word = words[i];
                if (!_invertedIndexData.Index.TryGetValue(word, out var currentWordPostings))
                {
                    return Enumerable.Empty<string>();
                }

                var stillMatchingDocs = new List<string>();
                foreach (var docId in matchingDocs)
                {
                    if (currentWordPostings.ContainsKey(docId))
                    {
                        var prevWordPositions = _invertedIndexData.Index[words[i - 1]][docId];
                        var currentWordPositions = currentWordPostings[docId];
                        
                        if (prevWordPositions.Any(pos => currentWordPositions.Contains(pos + 1)))
                        {
                            stillMatchingDocs.Add(docId);
                        }
                    }
                }
                matchingDocs = stillMatchingDocs;

                if (!matchingDocs.Any()) break;
            }
            
            return matchingDocs;
        }

        private void AddContentToIndex(string content, string documentIdentifier)
        {
            var tokensWithPositions = _tokenizer.TokenizeWithPositions(content);
            AddTokensToIndex(tokensWithPositions, documentIdentifier);
        }

        private void AddTokensToIndex(IEnumerable<(string Token, int Position)> tokensWithPositions, string documentIdentifier)
        {
            foreach (var (token, position) in tokensWithPositions)
            {
                AddTokenToIndex(token, documentIdentifier, position);
            }
        }

        public IEnumerable<string> GetAllDocuments()
        {
            return _invertedIndexData.Index
                .SelectMany(kvp => kvp.Value.Keys)
                .Distinct();
        }
    }
}