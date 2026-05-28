using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public  class GameAttemptQuestionConfiguration : IEntityTypeConfiguration<GameAttemptQuestion>
	{
		public void Configure(EntityTypeBuilder<GameAttemptQuestion> builder)
		{
			builder.ToTable("GameAttemptQuestions", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.GameAttemptId)
				.IsRequired();

			builder.Property(x => x.QuestionsTemplatesId)
				.IsRequired();

			builder.Property(x => x.IsAnswered)
				.IsRequired();

			builder.Property(x => x.CreatedOn)
				.HasColumnType("datetime")
				.IsRequired();

			builder.HasMany(x => x.Options)
				.WithOne(x => x.GameAttemptQuestion)
				.HasForeignKey(x => x.GameAttemptQuestionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
