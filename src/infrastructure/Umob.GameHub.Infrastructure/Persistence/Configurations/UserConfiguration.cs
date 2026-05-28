using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public sealed class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users", "dbo");

			builder.HasKey(user => user.Id);

			builder.Property(user => user.Id)
				.ValueGeneratedOnAdd();

			builder.Property(user => user.Username)
				.HasColumnName("Username")
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(user => user.Email)
				.HasColumnName("Email")
				.HasMaxLength(250)
				.IsRequired();

			builder.HasIndex(user => user.Email)
				.IsUnique();

			builder.Property(user => user.PasswordHash)
				.HasColumnName("PasswordHash")
				.HasMaxLength(500)
				.IsRequired();

			builder.Property(user => user.CreatedOn)
				.HasColumnName("CreatedOn")
				.IsRequired();

			builder.Property(user => user.ModifiedOn)
				.HasColumnName("ModifiedOn");
		}
	}
}
