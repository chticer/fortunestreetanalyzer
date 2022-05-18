namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class TurnAfterRoll
    {
        public long ID { get; set; }
        public long AnalyzerInstanceID { get; set; }
        public long CharacterID { get; set; }
        public long SpaceID { get; set; }
        public byte DieRollValue { get; set; }
        public DateTime TimestampAdded { get; set; }
    }
}
