using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreXF.Store.Data.Entities.Configurations
{
    public class ProductMetaDataConfiguration : IEntityTypeConfiguration<ProductMetaData>
    {
        public void Configure(EntityTypeBuilder<ProductMetaData> builder)
        {
            builder.ToTable("ProductsMetaData", "CoreXF");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();


            builder.Property<string>("MetaDataType").HasMaxLength(400);
            builder.HasDiscriminator<string>("MetaDataType");

            builder.Property(x => x.Price).IsRequired().HasDefaultValue(0).HasColumnType("decimal(18,4)");

            builder.HasOne(x => x.Product).WithOne(x => x.MetaData).HasForeignKey<ProductMetaData>("ProductId").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
