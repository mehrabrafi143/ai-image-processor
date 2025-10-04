namespace ai_image_processor.Models
{
    public class ProcessingResult
    {
        public string Classification { get; set; }
        public double Confidence { get; set; }
        public List<DetectedObject> Objects { get; set; }
        public double ProcessingTime { get; set; }
    }
}
