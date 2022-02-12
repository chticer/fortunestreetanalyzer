namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class Districts
    {
        public Districts()
        {
            Spaces = new HashSet<Spaces>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<Spaces> Spaces { get; set; }
    }
}
