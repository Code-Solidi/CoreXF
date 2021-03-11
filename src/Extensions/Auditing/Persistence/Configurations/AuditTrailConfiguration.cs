using Auditing.Domain.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auditing.Persistence.Configurations
{
    public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
    {
        public void Configure(EntityTypeBuilder<AuditTrail> builder)
        {
            builder.ToTable("AuditTrails", "Auditing");

            builder.Property(c => c.Who).IsRequired().HasMaxLength(250);
            builder.Property(c => c.When).IsRequired();
            builder.Property(c => c.What).IsRequired();
            builder.Property(c => c.Type).IsRequired().HasMaxLength(500);
            builder.Property(c => c.Identity).IsRequired().HasMaxLength("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx".Length);

            builder.HasKey(nameof(AuditTrail.Who), nameof(AuditTrail.When), nameof(AuditTrail.What), nameof(AuditTrail.Type), nameof(AuditTrail.Identity));
            builder.Property(c => c.MadeBy).HasMaxLength(250);
        }
    }
}