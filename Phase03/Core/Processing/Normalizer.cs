using System.Text.RegularExpressions;
using SearchEngine.Core.Interface;

namespace SearchEngine.Core.Processing
{
    public class Normalizer : INormalizer
    {
        public string Normalize(string text)
        {
            var withoutPunctuation = RemovePunctuation(text);
            var withConsolidatedWhitespace = ConsolidateWhitespace(withoutPunctuation);
            var uppercased = ConvertToUppercase(withConsolidatedWhitespace);
            return uppercased;
        }

        public IEnumerable<string> Normalize(IEnumerable<string> data)
        {
            foreach (string item in data)
            {
                yield return Normalize(item);
            }
        }
        private string RemovePunctuation(string text)
        {
            return Regex.Replace(text, @"[^\w\s]", " ");
        }

        private string ConsolidateWhitespace(string text)
        {
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private string ConvertToUppercase(string text)
        {
            return text.ToUpperInvariant();
        }
    }
}