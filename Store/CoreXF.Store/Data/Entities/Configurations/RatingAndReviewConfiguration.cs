using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreXF.Store.Data.Entities.Configurations
{
    public class RatingAndReviewConfiguration : IEntityTypeConfiguration<RatingAndReview>
    {
        public void Configure(EntityTypeBuilder<RatingAndReview> builder)
        {
            builder.ToTable("ProductRatings", "CoreXF");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Property(x => x.Rating).IsRequired();
            builder.Property(x => x.RatedOnUtc).IsRequired();

            builder.HasOne(x => x.RatedBy);
            builder.HasOne(x => x.Product).WithMany().IsRequired();
        }
    }
}
