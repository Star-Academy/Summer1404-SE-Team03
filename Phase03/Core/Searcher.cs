using SearchEngine.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine.Core
{
    public class Searcher
    {
        private readonly InvertedIndexManager _indexManager; // Depend on an interface

        public Searcher(InvertedIndexManager indexManager)
        {
            _indexManager = indexManager;
        }

        public IEnumerable<string> SmartSearch(SearchQuery query)
        {
            var baseResults = GetMustIncludeResults(query.MustInclude);

            if (query.AtLeastOne.Any())
            {
                var orResults = query.AtLeastOne
                                     .SelectMany(GetDocumentsForTerm)
                                     .ToHashSet();
                baseResults.IntersectWith(orResults);
            }

            foreach (var term in query.MustExclude)
            {
                baseResults.ExceptWith(GetDocumentsForTerm(term));
            }

            return baseResults;
        }

        private HashSet<string> GetMustIncludeResults(IEnumerable<string> mustIncludeTerms)
        {
            if (!mustIncludeTerms.Any())
            {
                return new HashSet<string>(_indexManager.GetAllDocuments());
            }

            var initialSet = new HashSet<string>(GetDocumentsForTerm(mustIncludeTerms.First()));

            return mustIncludeTerms.Skip(1).Aggregate(initialSet, (current, term) =>
            {
                current.IntersectWith(GetDocumentsForTerm(term));
                return current;
            });
        }

        private IEnumerable<string> GetDocumentsForTerm(string term)
        {
            return _indexManager.GetDocumentsForToken(term.Trim('\"'));
        }
    }
}