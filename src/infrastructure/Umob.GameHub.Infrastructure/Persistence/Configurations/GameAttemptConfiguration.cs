using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public class GameAttemptConfiguration : IEntityTypeConfiguration<GameAttempt>
	{
		public void Configure(EntityTypeBuilder<GameAttempt> builder)
		{
			builder.ToTable("GameAttempts", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.UserId)
				.IsRequired();

			builder.Property(x => x.Score)
				.IsRequired();

			builder.Property(x => x.IsWon)
				.IsRequired(false);

			builder.Property(x => x.StartedOn)
				.HasColumnType("datetime")
				.IsRequired();

			builder.Property(x => x.EndedOn)
				.HasColumnType("datetime")
				.IsRequired(false);

			builder.Property(x => x.DurationSeconds)
				.IsRequired();

			builder.HasMany(x => x.Questions)
				.WithOne(x => x.GameAttempt)
				.HasForeignKey(x => x.GameAttemptId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
