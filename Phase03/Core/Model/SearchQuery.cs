namespace SearchEngine.Core.Model
{
    public class SearchQuery
    {
        public IEnumerable<string> MustInclude { get; }
        public IEnumerable<string> AtLeastOne { get; }
        public IEnumerable<string> MustExclude { get; }

        public SearchQuery(IEnumerable<string> mustInclude, IEnumerable<string> atLeastOne, IEnumerable<string> mustExclude)
        {
            MustInclude = mustInclude;
            AtLeastOne = atLeastOne;
            MustExclude = mustExclude;
        }
    }
}