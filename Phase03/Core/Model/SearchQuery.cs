using SearchEngine.Core.Interface;

namespace SearchEngine.Core.Model
{
    public class SearchQuery : ISearchQuery
    {
        public IEnumerable<string> MustInclude { get; set; }
        public IEnumerable<string> AtLeastOne { get; set; }
        public IEnumerable<string> MustExclude { get; set; }

        public SearchQuery()
        {
            MustInclude = new string[0];
            AtLeastOne = new string[0];
            MustExclude = new string[0];
        }

        public SearchQuery(IEnumerable<string> mustInclude, IEnumerable<string> atLeastOne, IEnumerable<string> mustExclude)
        {
            MustInclude = mustInclude;
            AtLeastOne = atLeastOne;
            MustExclude = mustExclude;
        }
    }
}