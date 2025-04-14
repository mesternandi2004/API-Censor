using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using CENZURAZO_CQW1QQ_MESTER.Models;
using System.Xml;
using CENZURAZO_CQW1QQ_MESTER.Data;

namespace CENZURAZO_CQW1QQ_MESTER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CensorshipController : ControllerBase
    {
        ICensorRepository repo;

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

        [HttpPost]
        public void GetReplacementData([FromBody] ReplacementData data)
        {
            this.repo.Create(data);
        }

        [HttpPut]
        public void EditReplacement([FromBody] ReplacementData data)
        {
            this.repo.Update(data);
        }

        [HttpDelete("{id}")]
        public void DeleteReplacementdata(int id)
        {
            this.repo.Delete(id);
        }
    }
}
