using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public class QuestionTemplateConfiguration : IEntityTypeConfiguration<QuestionTemplate>
	{
		public void Configure(EntityTypeBuilder<QuestionTemplate> builder)
		{
			builder.ToTable("QuestionsTemplates", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.TextTemplate)
				.HasMaxLength(500)
				.IsRequired();

			builder.Property(x => x.IsActive)
				.IsRequired();

			builder.Property(x => x.CreatedOn)
				.HasColumnType("datetime")
				.IsRequired();
		
			builder.Property(x => x.StrategyType)
			 .HasMaxLength(250)
			 .IsRequired();

			builder.HasMany(x => x.GbfsFields)
				.WithOne(x => x.QuestionTemplate)
				.HasForeignKey(x => x.QuestionTemplateId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
