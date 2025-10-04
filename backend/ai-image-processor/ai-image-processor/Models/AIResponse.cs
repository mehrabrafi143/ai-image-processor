namespace ai_image_processor.Models
{
    public class AIResponse
    {
        public string Classification { get; set; }
        public double Confidence { get; set; }
        public List<AIObject> Objects { get; set; }
        public double ProcessingTime { get; set; }
    }
}