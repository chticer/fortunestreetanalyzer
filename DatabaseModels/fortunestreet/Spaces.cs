namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Spaces
    {
        public Spaces()
        {
            SpaceConstraintsFrom = new HashSet<SpaceConstraints>();
            SpaceConstraintsTo = new HashSet<SpaceConstraints>();
            SpaceConstraints = new HashSet<SpaceConstraints>();
        }

        public long ID { get; set; }
        public long RuleID { get; set; }
        public long BoardID { get; set; }
        public long SpaceTypeID { get; set; }
        public long? ShopID { get; set; }
        public long? DistrictID { get; set; }
        public string AdditionalProperties { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual Boards Board { get; set; }
        public virtual Districts District { get; set; }
        public virtual Rules Rule { get; set; }
        public virtual Shops Shop { get; set; }
        public virtual SpaceTypes SpaceType { get; set; }
        public virtual ICollection<SpaceConstraints> SpaceConstraintsFrom { get; set; }
        public virtual ICollection<SpaceConstraints> SpaceConstraintsTo { get; set; }
        public virtual ICollection<SpaceConstraints> SpaceConstraints { get; set; }
    }
}
