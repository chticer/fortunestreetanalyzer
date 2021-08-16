using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Colors
    {
        public Colors()
        {
            CharacterColorCrosslists = new HashSet<CharacterColorCrosslist>();
            GameSettings = new HashSet<GameSettings>();
        }

        public long ID { get; set; }
        public string MiiColor { get; set; }
        public string GameColor { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<CharacterColorCrosslist> CharacterColorCrosslists { get; set; }
        public virtual ICollection<GameSettings> GameSettings { get; set; }
    }
}
