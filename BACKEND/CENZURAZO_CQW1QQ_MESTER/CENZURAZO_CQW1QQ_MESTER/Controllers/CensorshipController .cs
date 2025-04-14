using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CENZURAZO_CQW1QQ_MESTER.Models;
using System.Xml;

namespace CENZURAZO_CQW1QQ_MESTER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CensorshipController : ControllerBase
    {
        // Post Method
        [HttpPost]
        public ActionResult<CensorshipResult> Post([FromBody] CensorshipRequest request)
        {
            // Blacklist and text changes
            var blacklist = ParseBlacklist(request.BlacklistText);
            var modifiedText = request.InputText;
            var usedAlternatives = new Dictionary<string, HashSet<string>>();
            var replacements = new List<ReplacementData>();

            // Balcklist words changes
            foreach (var entry in blacklist)
            {
                var word = entry.Key;
                var alternatives = entry.Value.ToList();
                var altIndex = 0;

                // Regex pattern for whole word match (case-insensitive)
                string pattern = $@"\b{Regex.Escape(word)}\b";
                var matches = Regex.Matches(modifiedText, pattern, RegexOptions.IgnoreCase);

                // Selecting alternatives cyclically
                foreach (Match match in matches)
                {
                    string matchedWord = match.Value;

                    
                    var available = alternatives.Where(a =>
                        !usedAlternatives.TryGetValue(matchedWord, out var set) || !set.Contains(a)
                    ).ToList();

                    if (available.Count == 0)
                    {
                        usedAlternatives[matchedWord] = new HashSet<string>();
                        available = alternatives.ToList();
                    }

                    string selected = available[altIndex % available.Count];
                    altIndex++;

                    // Handling lowercase and uppercase letters
                    string finalReplacement = MatchCase(matchedWord, selected);

                    // Word exchange in the text
                    modifiedText = Regex.Replace(modifiedText,
                        $@"\b{Regex.Escape(matchedWord)}\b",
                        finalReplacement,
                        RegexOptions.IgnoreCase,
                        TimeSpan.FromSeconds(1));

                    // Save to avoid repetition
                    if (!usedAlternatives.ContainsKey(matchedWord))
                        usedAlternatives[matchedWord] = new HashSet<string>();

                    usedAlternatives[matchedWord].Add(selected);

                    replacements.Add(new ReplacementData
                    {
                        OriginalWord = matchedWord,
                        ReplacedWord = finalReplacement
                    });
                }
            }

            // Word Frequency Calculation
            var originalCounts = WordCount(request.InputText);
            var modifiedCounts = WordCount(modifiedText);

            // Send Reply
            return Ok(new CensorshipResult
            {
                ModifiedText = modifiedText,
                Replacements = replacements,
                OriginalWordCount = originalCounts,
                ModifiedWordCount = modifiedCounts
            });
        }
        [HttpPost("add")]
        public ActionResult AddToBlacklist([FromBody] BlacklistWordRequest request)
        {
            var blacklist = GetBlacklist();  // A feketelistát valahonnan le kell kérni
            if (blacklist.ContainsKey(request.Word))
            {
                return BadRequest("A szó már létezik a feketelistán.");
            }

            blacklist[request.Word] = request.Alternatives;
            SaveBlacklist(blacklist);  // A feketelista mentése
            return Ok("Szó hozzáadva a feketelistához.");

        }

        [HttpDelete("delete/{word}")]
        public ActionResult DeleteFromBlacklist(string word)
        {
            var blacklist = GetBlacklist();
            if (!blacklist.ContainsKey(word))
            {
                return NotFound("A szó nem található a feketelistán.");
            }

            blacklist.Remove(word);
            SaveBlacklist(blacklist);  // A feketelista mentése
            return Ok("Szó törölve a feketelistáról.");
        }

        [HttpPut("update/{word}")]
        public ActionResult UpdateBlacklistWord(string word, [FromBody] BlacklistWordRequest request)
        {
            var blacklist = GetBlacklist();
            if (!blacklist.ContainsKey(word))
            {
                return NotFound("A szó nem található a feketelistán.");
            }

            blacklist[word] = request.Alternatives;
            SaveBlacklist(blacklist);  // A feketelista mentése
            return Ok("Szó frissítve a feketelistán.");
        }

        private Dictionary<string, List<string>> GetBlacklist()
        {
            // A feketelista beolvasása, például egy JSON fájlból
            string json = System.IO.File.ReadAllText("blacklist.json");
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }

        private void SaveBlacklist(Dictionary<string, List<string>> blacklist)
        {
            // A feketelista mentése, például JSON fájlba
            string json = JsonConvert.SerializeObject(blacklist, Formatting.Indented);
            System.IO.File.WriteAllText("blacklist.json", json);
        }


        // Method for : It breaks down the blacklist based on the @ character and organizes it into a dictionary, where the key is the word and the values ​​are the alternatives that can be used to replace the word.
        private Dictionary<string, List<string>> ParseBlacklist(string text)
        {
            var dict = new Dictionary<string, List<string>>();

            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Trim().Split('@');
                if (parts.Length == 2)
                {
                    var word = parts[0].Trim();
                    var alternatives = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                    dict[word] = alternatives;
                }
            }

            return dict;
        }

        //Method for:Counts the occurrences of words in a given text.
        private Dictionary<string, int> WordCount(string input)
        {
            var words = Regex.Matches(input.ToLower(), @"\b\w+\b");
            return words
                .GroupBy(w => w.Value)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Methods for: Ensures that the replaced word retains the case of the original word
        private string MatchCase(string original, string replacement)
        {
            if (string.IsNullOrEmpty(original)) return replacement;
            if (char.IsUpper(original[0]))
                return char.ToUpper(replacement[0]) + replacement.Substring(1);
            else
                return replacement.ToLower();
        }
    }
}
