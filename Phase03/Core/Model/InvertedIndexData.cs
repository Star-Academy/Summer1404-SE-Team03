namespace SearchEngine.Core.Model
{
    public class InvertedIndexData
    {
        public Dictionary<string, HashSet<string>> Index { get; }

        public InvertedIndexData()
        {
            Index = new Dictionary<string, HashSet<string>>();
        }
    }
}