using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Boards
    {
        public Boards()
        {
            BoardCharacterCrosslists = new HashSet<BoardCharacterCrosslist>();
            BoardGoals = new HashSet<BoardCharacteristics>();
            GameSettings = new HashSet<GameSettings>();
            Spaces = new HashSet<Spaces>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<BoardCharacterCrosslist> BoardCharacterCrosslists { get; set; }
        public virtual ICollection<BoardCharacteristics> BoardGoals { get; set; }
        public virtual ICollection<GameSettings> GameSettings { get; set; }
        public virtual ICollection<Spaces> Spaces { get; set; }
    }
}
