namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class CurrentAnalyzerInstanceLogsTVF
    {
        public long ID { get; set; }
        public long AnalyzerInstanceID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime TimestampAdded { get; set; }
    }
}
