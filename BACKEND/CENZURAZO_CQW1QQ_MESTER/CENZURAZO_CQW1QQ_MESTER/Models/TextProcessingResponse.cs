namespace CENZURAZO_CQW1QQ_MESTER.Models
{
    public class TextProcessingResponse
    {

        public string OriginalText { get; set; }
        public string ProcessedText { get; set; }
        public Dictionary<string, int> OriginalWordCounts { get; set; }
        public Dictionary<string, int> ProcessedWordCounts { get; set; }
        public List<string> BlacklistWords { get; set; } 

    }
}
