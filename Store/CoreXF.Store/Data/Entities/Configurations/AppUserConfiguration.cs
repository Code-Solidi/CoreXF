using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreXF.Store.Data.Entities.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.RegisteredOn).IsRequired();
            //builder.HasMany(x => x.Extensions).WithOne(x => x.Owner).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
