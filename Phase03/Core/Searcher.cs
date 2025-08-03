using SearchEngine.Core.Model;

namespace SearchEngine.Core
{
    public class Searcher
    {
        private readonly InvertedIndexManager _indexManager;

        public Searcher(InvertedIndexManager indexManager)
        {
            _indexManager = indexManager;
        }

        public IEnumerable<string> SmartSearch(SearchQuery query)
        {
            var mustIncludeResults = new HashSet<string>(_indexManager.GetAllDocuments());
            if (query.MustInclude.Any())
            {
                mustIncludeResults.Clear();
                var firstTerm = query.MustInclude.First();
                var cleanTerm = firstTerm.Trim('\"');
                mustIncludeResults.UnionWith(_indexManager.GetDocumentsForToken(cleanTerm));

                foreach (var term in query.MustInclude.Skip(1))
                {
                    cleanTerm = term.Trim('\"');
                    mustIncludeResults.IntersectWith(_indexManager.GetDocumentsForToken(cleanTerm));
                }
            }

            var atLeastOneResults = new HashSet<string>();
            foreach (var term in query.AtLeastOne)
            {
                var cleanTerm = term.Trim('\"');
                atLeastOneResults.UnionWith(_indexManager.GetDocumentsForToken(cleanTerm));
            }

            if (query.AtLeastOne.Any())
            {
                mustIncludeResults.IntersectWith(atLeastOneResults);
            }

            foreach (var term in query.MustExclude)
            {
                var cleanTerm = term.Trim('\"');
                mustIncludeResults.ExceptWith(_indexManager.GetDocumentsForToken(cleanTerm));
            }

            return mustIncludeResults;
        }
    }
}