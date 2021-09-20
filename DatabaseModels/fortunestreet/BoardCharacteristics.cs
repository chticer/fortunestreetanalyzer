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
        public short ReadyCashStart { get; set; }
        public short SalaryStart { get; set; }
        public byte SalaryIncrease { get; set; }
        public byte MaxDieRoll { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Board { get; set; }
        public virtual Rules Rule { get; set; }
    }
}
