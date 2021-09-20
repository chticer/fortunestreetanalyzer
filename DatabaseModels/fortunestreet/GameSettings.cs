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

        public virtual Boards Board { get; set; }
        public virtual Colors Color { get; set; }
        public virtual Rules Rule { get; set; }
    }
}
