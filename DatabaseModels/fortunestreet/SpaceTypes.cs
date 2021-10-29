using System;
using System.Collections.Generic;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public partial class SpaceTypes
    {
        public SpaceTypes()
        {
            Spaces = new HashSet<Spaces>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimestampAdded { get; set; }

        public virtual ICollection<Spaces> Spaces { get; set; }
    }
}
