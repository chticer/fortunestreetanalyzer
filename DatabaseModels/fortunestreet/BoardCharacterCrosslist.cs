namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class BoardCharacterCrosslist
    {
        public long ID { get; set; }
        public long BoardID { get; set; }
        public long CharacterID { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Board { get; set; }
        public virtual Characters Character { get; set; }
    }
}
