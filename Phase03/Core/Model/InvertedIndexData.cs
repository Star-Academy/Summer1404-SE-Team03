using System.Collections.Generic;

namespace SearchEngine.Core.Model
{
    public class InvertedIndexData
    {
        public Dictionary<string, Dictionary<string, HashSet<int>>> Index { get; }

        public InvertedIndexData()
        {
            Index = new Dictionary<string, Dictionary<string, HashSet<int>>>();
        }
    }
}