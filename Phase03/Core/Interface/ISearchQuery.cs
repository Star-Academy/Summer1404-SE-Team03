namespace SearchEngine.Core.Interface
{
    public interface ISearchQuery
    {
        public IEnumerable<string> MustInclude { get; set; }
        public IEnumerable<string> AtLeastOne { get; set; }
        public IEnumerable<string> MustExclude { get; set; }
    }
}