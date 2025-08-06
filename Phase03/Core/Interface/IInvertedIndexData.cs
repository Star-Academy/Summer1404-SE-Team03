namespace SearchEngine.Core.Interface
{
    public interface IInvertedIndexData
    {
        public Dictionary<string, HashSet<string>> Index { get; }
    }
}