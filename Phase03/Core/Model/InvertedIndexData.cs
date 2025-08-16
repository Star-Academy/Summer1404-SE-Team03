using SearchEngine.Core.Interface;

namespace SearchEngine.Core.Model
{
    public class InvertedIndexData : IInvertedIndexData
    {
        public Dictionary<string, HashSet<string>> Index { get; }

        public InvertedIndexData()
        {
            Index = new Dictionary<string, HashSet<string>>();
        }
    }
}