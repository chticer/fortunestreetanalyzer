using System;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class BoardCharacteristics
    {
        public long ID { get; set; }
        public long RuleID { get; set; }
        public long BoardID { get; set; }
        public byte StandingThreshold { get; set; }
        public short NetWorthThreshold { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Board { get; set; }
        public virtual Rules Rule { get; set; }
    }
}
