namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class SpaceConstraints
    {
        public long ID { get; set; }
        public long SpaceID { get; set; }
        public long SpaceIDFrom { get; set; }
        public long SpaceIDTo { get; set; }
        public byte SpaceLayoutIndex { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Spaces Space { get; set; }
        public virtual Spaces SpaceFrom { get; set; }
        public virtual Spaces SpaceTo { get; set; }
    }
}
