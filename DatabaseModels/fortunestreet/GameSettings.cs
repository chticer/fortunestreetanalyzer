using System;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class GameSettings
    {
        public long ID { get; set; }
        public long AnalyzerInstanceID { get; set; }
        public long RuleID { get; set; }
        public long BoardID { get; set; }
        public long MiiColorID { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Boards { get; set; }
        public virtual Colors Colors { get; set; }
        public virtual Rules Rules { get; set; }
    }
}
