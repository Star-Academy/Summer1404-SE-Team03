using SearchEngine.Core.Interface;
using SearchEngine.Core.Model;

namespace SearchEngine.Core
{
    public class Searcher : ISearcher
    {
        public IEnumerable<string> SmartSearch(ISearchQuery query, IInvertedIndex _indexmanager)
        {
            var result = InitializeResultSet(query, _indexmanager);
            result = FilterByAtLeastOne(result, query.AtLeastOne, _indexmanager);
            result = FilterByMustExclude(result, query.MustExclude, _indexmanager);
            return result;
        }

        public IEnumerable<string> InitializeResultSet(ISearchQuery query, IInvertedIndex _indexmanager)
        {
            if (query.MustInclude.Any())
                return PerformAndSearch(query.MustInclude, _indexmanager);

            return _indexmanager.GetAllDocuments();
        }

        public IEnumerable<string> PerformAndSearch(IEnumerable<string> mustInclude, IInvertedIndex _indexmanager)
        {
            IEnumerable<string> result = null;
            foreach (var word in mustInclude)
            {
                var docs = _indexmanager.GetDocumentsForToken(word.ToUpperInvariant());
                result = result == null ? docs : result.Intersect(docs);
            }
            return result ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> FilterByAtLeastOne(IEnumerable<string> currentResult, IEnumerable<string> atLeastOne, IInvertedIndex _indexmanager)
        {
            if (!atLeastOne.Any())
            {
                return currentResult;
            }
            var orSet = atLeastOne.SelectMany(w => _indexmanager.GetDocumentsForToken(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Intersect(orSet);
        }

        public IEnumerable<string> FilterByMustExclude(IEnumerable<string> currentResult, IEnumerable<string> mustExclude, IInvertedIndex _indexmanager)
        {
            if (!mustExclude.Any())
            {
                return currentResult;
            }
            var notSet = mustExclude.SelectMany(w => _indexmanager.GetDocumentsForToken(w.ToUpperInvariant())).ToHashSet();
            return currentResult.Except(notSet);
        }
    }
}