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


        // Fő feldolgozó metódus: cenzúrázza a szöveget és előállítja a választ
        // Main processing method: censors text and builds response
        public TextProcessingResponse Process(string input)
        {
            string processed = input;
            usedBlacklistWords.Clear(); 

            foreach (var rule in blacklist)
            {
                if (string.IsNullOrWhiteSpace(rule.Word) || rule.Alternatives == null || rule.Alternatives.Count == 0)
                    continue;

                // Regex minta teljes szavakra (kis/nagybetű független)
                // Regex pattern for whole words only (case-insensitive)
                string pattern = $@"\b{Regex.Escape(rule.Word)}\b";
                processed = Regex.Replace(processed, pattern, match =>
                {
                    var alt = GetNextAlternative(rule); //Válassz egy alternatívát // Pick a random alternative
                    UpdateCounts(match.Value, alt);     // Frissítsd a szógyakoriságokat // Update word counts
                    usedBlacklistWords.Add(rule.Word); // Jegyezd meg, hogy ezt a szót lecseréltük // Mark this word as used

                    return WrapReplacement(match.Value, alt);  // Cseréld le badge formátumban // Replace it with a badge-wrapped
                }, RegexOptions.IgnoreCase);
            }

            CountWords(input, originalCounts);        // Számolja az eredeti szavakat // Count original words
            CountWords(processed, processedCounts);  // Számolja a feldolgozott szavakat // Count processed words

            return new TextProcessingResponse
            {
                OriginalText = input,
                ProcessedText = processed,
                OriginalWordCounts = originalCounts,
                ProcessedWordCounts = processedCounts,
                BlacklistWords = usedBlacklistWords.ToList() 
            };
        }

        // Lecserélt szó HTML badge-ként formázása (eredeti + alternatíva)
        // Formats the replacement: original word in red badge, new one in green
        private string WrapReplacement(string original, string alt)
        {
            bool isUpper = char.IsUpper(original[0]);
            string formattedAlt = isUpper ? Capitalize(alt) : alt.ToLower();

            return $"<span class='badge bg-danger'>{original}</span> <span class='badge bg-success'>{formattedAlt}</span>";
        }

        // Alternatíva nagy kezdőbetűvel, ha az eredeti is nagy
        // Capitalizes the replacement if the original started with a capital letter
        private string Capitalize(string word)
        {
            if (string.IsNullOrEmpty(word)) return word;
            return char.ToUpper(word[0]) + word.Substring(1).ToLower();
        }

        // Visszaad egy véletlenszerű alternatívát a listából
        // Returns a random alternative from the list
        private string GetNextAlternative(ReplacementData rule)
        {
            int index = rng.Next(rule.Alternatives.Count);
            return rule.Alternatives.ElementAt(index).Alternative;
        }

        // Szavak számolása egy szövegből, kisbetűsítve
        // Counts how often each word appears (case-insensitive)
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

        // Frissíti az eredeti és a lecserélt szavak előfordulását
        // Updates frequency counts for both original and replaced words
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
