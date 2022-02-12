namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Shops
    {
        public Shops()
        {
            Spaces = new HashSet<Spaces>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<Spaces> Spaces { get; set; }
    }
}
