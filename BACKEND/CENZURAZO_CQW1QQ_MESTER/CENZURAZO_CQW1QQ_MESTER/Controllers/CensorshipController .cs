﻿using Microsoft.AspNetCore.Mvc;
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

        // Returns all ReplacementData records
        [HttpGet]
        public IEnumerable<ReplacementData> GetReplacementData()
        {
            return this.repo.Read();
        }

        // Returns a specific ReplacementData by ID
        [HttpGet("{id}")]
        public ReplacementData? GetReplacementData(int id)
        {
            return this.repo.Read(id);
        }

        // Deletes a word from the blacklist by the word itself
        [HttpDelete("blacklist/{word}")]
        public IActionResult DeleteWord(string word)
        {
            var success = repo.DeleteByWord(word);
            if (!success)
                return NotFound("The given word was not found"); 

            return Ok();
        }

        // Retrieves all blacklisted words and their alternatives
        [HttpGet("blacklist")]
        public IActionResult GetBlacklist()
        {
            var blacklistData = this.repo.GetAll();
            return Ok(blacklistData.Select(item => new {
                word = item.Word,
                alternatives = item.Alternatives.Select(a => a.Alternative).ToList()
            }));
        }

        // Adds new blacklisted words and alternatives
        [HttpPost("blacklist")]
        public IActionResult CreateFromBlacklist([FromBody] Dictionary<string, List<string>> blacklist)
        {
            if (blacklist == null || !blacklist.Any())
            {
                return BadRequest(" Empty or invalid data received"); 
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

        // Updates existing blacklisted words or adds them if they don't exist
        [HttpPut("blacklist")]
        public IActionResult EditBlacklist([FromBody] Dictionary<string, List<string>> updates)
        {
            if (updates == null || !updates.Any())
                return BadRequest("No update data provided."); 

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

        
        // Processes the text: replaces blacklisted words with alternatives
        [HttpPost("process")]
        public ActionResult<TextProcessingResponse> ProcessText([FromBody] TextProcessingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Input text must not be empty"); 
            }

            var blacklist = this.repo.Read().ToList();

            var processor = new TextProcessor(blacklist);
            var response = processor.Process(request.Text);

            return Ok(response);
        }
    }
}
