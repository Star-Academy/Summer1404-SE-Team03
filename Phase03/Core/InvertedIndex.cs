using SearchEngine.Core.Model;
using SearchEngine.Core.Processing;

namespace SearchEngine.Core
{
    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<string>> _index = new Dictionary<string, HashSet<string>>();
        private readonly ITokenizer _tokenizer;

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public void AddDocument(string documentPath)
        {
            if (!TryReadContent(documentPath, out var content))
            {
                return;
            }

            var tokens = _tokenizer.Tokenize(content);

            foreach (var token in tokens)
            {
                AddTokenToIndex(token, documentPath);
            }
        }

        private bool TryReadContent(string documentPath, out string content)
        {
            try
            {
                content = File.ReadAllText(documentPath);
                return true;
            }
            catch (IOException)
            {
                content = null;
                return false;
            }
        }

        private void AddTokenToIndex(string token, string documentPath)
        {
            if (!_index.TryGetValue(token, out var docSet))
            {
                docSet = new HashSet<string>();
                _index[token] = docSet;
            }
            docSet.Add(documentPath);
        }

        public IEnumerable<string> SmartSearch(SearchQuery query)
        {
            var result = InitializeResultSet(query);
            result = FilterByAtLeastOne(result, query.AtLeastOne);
            result = FilterByMustExclude(result, query.MustExclude);
            return result;
        }

        private IEnumerable<string> InitializeResultSet(SearchQuery query)
        {
            if (query.MustInclude.Any())
            {
                return PerformAndSearch(query.MustInclude);
            }
            
            return _index.Values.SelectMany(set => set).Distinct();
        }

        private IEnumerable<string> PerformAndSearch(IEnumerable<string> mustInclude)
        {
            IEnumerable<string> result = null;
            foreach (var word in mustInclude)
            {
                var docs = Search(word.ToUpperInvariant());
                result = result == null ? docs : result.Intersect(docs);
            }
            return result ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> FilterByAtLeastOne(IEnumerable<string> currentResult, IEnumerable<string> atLeastOne)
        {
            if (!atLeastOne.Any())
            {
                return currentResult;
            }
            var orSet = atLeastOne.SelectMany(w => Search(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Intersect(orSet);
        }

        private IEnumerable<string> FilterByMustExclude(IEnumerable<string> currentResult, IEnumerable<string> mustExclude)
        {
            if (!mustExclude.Any())
            {
                return currentResult;
            }
            var notSet = mustExclude.SelectMany(w => Search(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Except(notSet);
        }

        public IEnumerable<string> Search(string token)
        {
            if (_index.TryGetValue(token, out var docSet))
                return docSet;
            return Enumerable.Empty<string>();
        }
    }
}