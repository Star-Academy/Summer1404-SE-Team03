// File: Core/InvertedIndex.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Phase02.Core
{
    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<string>> _index = new Dictionary<string, HashSet<string>>();

        private static string Normalize(string text)
        {
            var noPunct = Regex.Replace(text, @"[^\w\s]", " ");
            var singleSpaced = Regex.Replace(noPunct, @"\s+", " ").Trim();
            return singleSpaced.ToUpperInvariant();
        }

        private static IEnumerable<string> Tokenize(string text)
        {
            var normalized = Normalize(text);
            return normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        public void AddDocument(string documentPath)
        {
            string content;
            try
            {
                content = File.ReadAllText(documentPath);
            }
            catch (IOException)
            {
                return;
            }

            var tokens = Tokenize(content);

            foreach (var token in tokens)
            {
                if (!_index.TryGetValue(token, out var docSet))
                {
                    docSet = new HashSet<string>();
                    _index[token] = docSet;
                }
                docSet.Add(documentPath);
            }
        }

        public IEnumerable<string> Search(string token)
        {
            var key = token.ToUpperInvariant();
            if (_index.TryGetValue(key, out var docSet))
                return docSet;
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> SmartSearch(
            IEnumerable<string> mustInclude,
            IEnumerable<string> atLeastOne,
            IEnumerable<string> mustExclude)
        {
            IEnumerable<string> result = null;

            foreach (var word in mustInclude)
            {
                var docs = Search(word);
                result = result == null ? docs : result.Intersect(docs);
            }

            if (result == null)
                result = _index.Values.SelectMany(s => s).Distinct();

            if (atLeastOne.Any())
            {
                var orSet = atLeastOne.SelectMany(w => Search(w)).ToHashSet();
                result = result.Intersect(orSet);
            }

            if (mustExclude.Any())
            {
                var notSet = mustExclude.SelectMany(w => Search(w)).ToHashSet();
                result = result.Except(notSet);
            }

            return result;
        }
    }
}
