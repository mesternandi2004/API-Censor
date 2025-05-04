using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CENZURAZO_CQW1QQ_MESTER.Models
{
    public class ReplacementData
    {
        [Key]
        public int ID { get; set; }
        public string Word { get; set; } = string.Empty;
        public ICollection<AlternativeWord> Alternatives { get; set; } = new List<AlternativeWord>();

        public int Count { get; set; }
    }
}
