using System;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class TurnOrderDetermination
    {
        public long ID { get; set; }
        public long AnalyzerInstanceID { get; set; }
        public long CharacterID { get; set; }
        public byte Value { get; set; }
        public DateTime TimestampAdded { get; set; }
    }
}
