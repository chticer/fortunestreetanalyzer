using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Rules
    {
        public Rules()
        {
            BoardGoals = new HashSet<BoardCharacteristics>();
            Spaces = new HashSet<Spaces>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<BoardCharacteristics> BoardGoals { get; set; }
        public virtual ICollection<Spaces> Spaces { get; set; }
    }
}
