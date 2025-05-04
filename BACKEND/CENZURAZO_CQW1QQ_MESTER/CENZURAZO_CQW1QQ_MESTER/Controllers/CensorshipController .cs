using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CENZURAZO_CQW1QQ_MESTER.Models;
using System.Xml;
using CENZURAZO_CQW1QQ_MESTER.Data;
using Microsoft.AspNetCore.Mvc;
using CENZURAZO_CQW1QQ_MESTER.Services;

namespace CENZURAZO_CQW1QQ_MESTER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CensorshipController : ControllerBase
    {
        private readonly ICensorRepository repo;


        public CensorshipController(ICensorRepository repo)
        {
            this.repo = repo;

        }
        [HttpGet]
        public IEnumerable<ReplacementData> GetReplacementData()
        {
            return this.repo.Read();
        }

        [HttpGet("{id}")]
        public ReplacementData? GetReplacementData(int id)
        {
            return this.repo.Read(id);
        }

        [HttpDelete("blacklist/{word}")]
        public IActionResult DeleteWord(string word)
        {
            var success = repo.DeleteByWord(word);
            if (!success)
                return NotFound("A megadott szó nem található.");

            return Ok();
        }

        [HttpGet("blacklist")]
        public IActionResult GetBlacklist()
        {
            var blacklistData = this.repo.GetAll();  // Visszaadja az összes feketelistás adatot
            return Ok(blacklistData.Select(item => new {
                word = item.Word,
                alternatives = item.Alternatives.Select(a => a.Alternative).ToList()
            }));
        }

        // Ezt meghagyjuk POST-nak, de külön route-on
        [HttpPost("blacklist")]
        public IActionResult CreateFromBlacklist([FromBody] Dictionary<string, List<string>> blacklist)
        {
            if (blacklist == null || !blacklist.Any())
            {
                return BadRequest("Üres vagy érvénytelen adat érkezett.");
            }

            foreach (var entry in blacklist)
            {
                var word = entry.Key;
                var alternativesList = entry.Value ?? new List<string>();

                var replacementData = new ReplacementData
                {
                    Word = word,
                    Alternatives = alternativesList
                        .Select(a => new AlternativeWord { Alternative = a })
                        .ToList()
                };

                this.repo.Create(replacementData);
            }

            return Ok(blacklist.Keys);
        }

        [HttpPut("blacklist")]
        public IActionResult EditBlacklist([FromBody] Dictionary<string, List<string>> updates)
        {
            if (updates == null || !updates.Any())
                return BadRequest("Nincs adat a frissítéshez.");

            foreach (var entry in updates)
            {
                var word = entry.Key;
                var alternatives = entry.Value;

                var existing = repo.Read().FirstOrDefault(x => x.Word == word);

                if (existing != null)
                {
                    existing.Alternatives = alternatives
                        .Select(alt => new AlternativeWord { Alternative = alt })
                        .ToList();
                    repo.Update(existing);
                }
                else
                {
                    var newData = new ReplacementData
                    {
                        Word = word,
                        Alternatives = alternatives
                            .Select(alt => new AlternativeWord { Alternative = alt })
                            .ToList()
                    };
                    repo.Create(newData);
                }
            }

            return Ok("Feketelista frissítve.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReplacementdata(int id)
        {
            this.repo.Delete(id);
            return Ok();
        }

        [HttpPost("process")]
        public ActionResult<TextProcessingResponse> ProcessText([FromBody] TextProcessingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("A bemeneti szöveg nem lehet üres.");
            }

            var blacklist = this.repo.Read().ToList();

            var processor = new TextProcessor(blacklist);
            var response = processor.Process(request.Text);

            return Ok(response);
        }
    }
}
