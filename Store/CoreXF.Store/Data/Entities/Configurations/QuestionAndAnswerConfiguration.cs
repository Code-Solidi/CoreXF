using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreXF.Store.Data.Entities.Configurations
{
    public class QuestionAndAnswerConfiguration : IEntityTypeConfiguration<QuestionAndAnswer>
    {
        public void Configure(EntityTypeBuilder<QuestionAndAnswer> builder)
        {
            builder.ToTable("ProductQuestionsAndAnswers", "CoreXF");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

            builder.Property(x => x.Question).IsRequired().HasMaxLength(500);
            builder.Property(x => x.AskedOnUtc).IsRequired();

            builder.HasOne(x => x.AskedBy);
            builder.HasOne(x => x.Product).WithMany().IsRequired();
        }
    }
}
