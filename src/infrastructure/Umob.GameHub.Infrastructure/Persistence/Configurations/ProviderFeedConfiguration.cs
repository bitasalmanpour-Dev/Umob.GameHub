using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Persistence.Configurations
{
	public sealed class ProviderFeedConfiguration
	: IEntityTypeConfiguration<ProviderFeed>
	{
		public void Configure(EntityTypeBuilder<ProviderFeed> builder)
		{
			builder.ToTable("ProviderFeeds", "dbo");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.ValueGeneratedOnAdd();

			builder.Property(x => x.FeedName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(x => x.Url)
				.IsRequired();

			builder.Property(x => x.ProviderId)
				.IsRequired();
		}
	}
}
