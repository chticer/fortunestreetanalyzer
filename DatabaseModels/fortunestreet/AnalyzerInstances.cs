namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class AnalyzerInstances
    {
        public long ID { get; set; }
        public long AnalyzerInstanceID { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime TimestampAdded { get; set; }
    }
}
