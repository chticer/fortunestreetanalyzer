using System;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class CharacterColorCrosslist
    {
        public long ID { get; set; }
        public long CharacterID { get; set; }
        public long ColorID { get; set; }
        public byte Position { get; set; }
        public byte? Priority { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Characters Character { get; set; }
        public virtual Colors Color { get; set; }
    }
}
