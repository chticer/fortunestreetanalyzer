using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class Shops
    {
        public Shops()
        {
            SpaceTypes = new HashSet<SpaceTypes>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public int Price { get; set; }
        public int MaxCapital { get; set; }
        public string District { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<SpaceTypes> SpaceTypes { get; set; }
    }
}
