using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public sealed class QuestionTemplateGbfsFieldConfiguration
	: IEntityTypeConfiguration<QuestionTemplateGbfsField>
	{
		public void Configure(EntityTypeBuilder<QuestionTemplateGbfsField> builder)
		{
			builder.ToTable("QuestionTemplateGbfsFields", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.QuestionTemplateId)
				.IsRequired();

			builder.Property(x => x.FeedName)
				.IsRequired();

			builder.Property(x => x.CollectionPath)
				.IsRequired();

			builder.Property(x => x.FieldName)
				.IsRequired();

			builder.Property(x => x.ProviderFeedsId)
					 .HasColumnName("ProviderFeedsId")
					 .IsRequired();

			builder.Property(x => x.CreatedOn)
				.HasColumnType("datetime")
				.IsRequired();

			builder.HasOne(x => x.QuestionTemplate)
				.WithMany(x => x.GbfsFields)
				.HasForeignKey(x => x.QuestionTemplateId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.ProviderFeed)
			   .WithMany()
			   .HasForeignKey(x => x.ProviderFeedsId)
			   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
