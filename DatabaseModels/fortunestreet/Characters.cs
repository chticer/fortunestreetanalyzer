using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Characters
    {
        public Characters()
        {
            BoardCharacterCrosslists = new HashSet<BoardCharacterCrosslist>();
            CharacterColorCrosslists = new HashSet<CharacterColorCrosslist>();
        }

        public long ID { get; set; }
        public string CharacterPortraitURI { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<BoardCharacterCrosslist> BoardCharacterCrosslists { get; set; }
        public virtual ICollection<CharacterColorCrosslist> CharacterColorCrosslists { get; set; }
    }
}
