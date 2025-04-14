using System.ComponentModel.DataAnnotations;

namespace CENZURAZO_CQW1QQ_MESTER.Models
{
    public class ReplacementData
    {
        [Key]
        public int ID { get; set; }
        public string Word { get; set; } = string.Empty;
        public List<string> Alternatives { get; set; } = new();
    }
}
