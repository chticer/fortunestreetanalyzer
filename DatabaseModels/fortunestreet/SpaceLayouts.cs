namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class SpaceLayouts
    {
        public long ID { get; set; }
        public long SpaceID { get; set; }
        public byte LayoutIndex { get; set; }
        public decimal CenterXFactor { get; set; }
        public decimal CenterYFactor { get; set; }
        public DateTime TimestampAdded { get; set; }
    }
}
