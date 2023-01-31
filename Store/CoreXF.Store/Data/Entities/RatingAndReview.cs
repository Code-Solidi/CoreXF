using System;

namespace CoreXF.Store.Data.Entities
{
    public class RatingAndReview
    {
        public Guid Id { get; set; }

        public uint Rating { get; set; }

        public AppUser RatedBy { get; set; }

        public DateTime RatedOnUtc { get; set; } = DateTime.UtcNow;

        public string Review { get; set; }

        public Product Product { get; set; }
    }
}