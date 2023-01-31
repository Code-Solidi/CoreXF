using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreXF.Store.Data.Entities.Configurations
{
    public class ExtensionConfiguration : IEntityTypeConfiguration<Extension>
    {
        public void Configure(EntityTypeBuilder<Extension> builder)
        {
            builder.Ignore(x => x.Overview);
            builder.Ignore(x => x.SemanticVersion);
            builder.Property(x => x.Rating).HasDefaultValue(0).HasColumnType("decimal(1,1)"); 
            builder.Property(x => x.Version).IsRequired().HasMaxLength(100);    // may contain alpha, beta, pre-release, RTM, etc.
            builder.HasOne(x => x.Owner).WithMany(x => x.Extensions).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
