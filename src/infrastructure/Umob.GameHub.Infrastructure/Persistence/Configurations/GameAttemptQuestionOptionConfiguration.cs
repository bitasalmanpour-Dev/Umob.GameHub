using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public class GameAttemptQuestionOptionConfiguration : IEntityTypeConfiguration<GameAttemptQuestionOption>
	{
		public void Configure(EntityTypeBuilder<GameAttemptQuestionOption> builder)
		{
			builder.ToTable("GameAttemptQuestionOptions", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.GameAttemptQuestionId)
				.IsRequired();

			builder.Property(x => x.OptionKey)
				.HasMaxLength(5)
				.IsRequired();

			builder.Property(x => x.OptionValue)
				.HasMaxLength(500)
				.IsRequired();

			builder.Property(x => x.IsCorrect)
				.IsRequired();

			builder.Property(x => x.CreatedOn)
				.HasColumnType("datetime")
				.IsRequired();

			builder.HasOne(x => x.GameAttemptQuestion)
			 .WithMany(x => x.Options)
			 .HasForeignKey(x => x.GameAttemptQuestionId)
			 .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
