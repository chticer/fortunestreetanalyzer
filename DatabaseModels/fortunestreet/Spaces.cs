using System;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Spaces
    {
        public long ID { get; set; }
        public long RuleID { get; set; }
        public long BoardID { get; set; }
        public long SpaceTypeID { get; set; }
        public decimal CenterX { get; set; }
        public decimal CenterY { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Board { get; set; }
        public virtual Rules Rule { get; set; }
        public virtual SpaceTypes SpaceType { get; set; }
    }
}
