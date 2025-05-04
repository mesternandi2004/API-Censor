using CENZURAZO_CQW1QQ_MESTER.Models;
using System.Text.RegularExpressions;

namespace CENZURAZO_CQW1QQ_MESTER.Services
{
    public class TextProcessor
    {
        private List<ReplacementData> blacklist;
        private Dictionary<string, int> originalCounts = new();
        private Dictionary<string, int> processedCounts = new();
        private Random rng = new();
        private HashSet<string> usedBlacklistWords = new(); 

        public TextProcessor(List<ReplacementData> blacklist)
        {
            this.blacklist = blacklist;
        }

        public TextProcessingResponse Process(string input)
        {
            string processed = input;
            usedBlacklistWords.Clear(); 

            foreach (var rule in blacklist)
            {
                if (string.IsNullOrWhiteSpace(rule.Word) || rule.Alternatives == null || rule.Alternatives.Count == 0)
                    continue;

                string pattern = $@"\b{Regex.Escape(rule.Word)}\b";
                processed = Regex.Replace(processed, pattern, match =>
                {
                    var alt = GetNextAlternative(rule);
                    UpdateCounts(match.Value, alt);
                    usedBlacklistWords.Add(rule.Word); 

                    return WrapReplacement(match.Value, alt);
                }, RegexOptions.IgnoreCase);
            }

            CountWords(input, originalCounts);
            CountWords(processed, processedCounts);

            return new TextProcessingResponse
            {
                OriginalText = input,
                ProcessedText = processed,
                OriginalWordCounts = originalCounts,
                ProcessedWordCounts = processedCounts,
                BlacklistWords = usedBlacklistWords.ToList() 
            };
        }


        private string WrapReplacement(string original, string alt)
        {
            bool isUpper = char.IsUpper(original[0]);
            string formattedAlt = isUpper ? Capitalize(alt) : alt.ToLower();

            return $"<span class='badge bg-danger'>{original}</span> <span class='badge bg-success'>{formattedAlt}</span>";
        }

        private string Capitalize(string word)
        {
            if (string.IsNullOrEmpty(word)) return word;
            return char.ToUpper(word[0]) + word.Substring(1).ToLower();
        }

        private string GetNextAlternative(ReplacementData rule)
        {
            int index = rng.Next(rule.Alternatives.Count);
            return rule.Alternatives.ElementAt(index).Alternative;
        }

        private void CountWords(string text, Dictionary<string, int> dict)
        {
            var words = Regex.Matches(text.ToLower(), @"\b\w+\b");
            foreach (Match match in words)
            {
                string word = match.Value;
                if (!dict.ContainsKey(word))
                    dict[word] = 0;
                dict[word]++;
            }
        }

        private void UpdateCounts(string original, string replacement)
        {
            string o = original.ToLower();
            string r = replacement.ToLower();

            if (!originalCounts.ContainsKey(o))
                originalCounts[o] = 0;
            originalCounts[o]++;

            if (!processedCounts.ContainsKey(r))
                processedCounts[r] = 0;
            processedCounts[r]++;
        }
    }
}
