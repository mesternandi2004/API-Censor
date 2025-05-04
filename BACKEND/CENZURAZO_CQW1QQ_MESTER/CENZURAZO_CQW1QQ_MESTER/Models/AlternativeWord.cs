using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CENZURAZO_CQW1QQ_MESTER.Models
{
    public class AlternativeWord
    {
        [Key]
        public int ID { get; set; }
        public string Alternative { get; set; }
        [JsonIgnore]
        public ReplacementData ReplacementData { get; set; }

    }
}
