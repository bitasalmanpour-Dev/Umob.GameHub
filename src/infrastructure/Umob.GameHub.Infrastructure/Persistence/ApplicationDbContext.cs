using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Domain.Entities;


namespace Umob.GameHub.Infrastructure.Persistence
{
	public sealed class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<User> Users => Set<User>();
		
		public DbSet<GbfsProvider> GbfsProviders => Set<GbfsProvider>();

		public DbSet<QuestionTemplate> QuestionTemplates => Set<QuestionTemplate>();

		public DbSet<QuestionTemplateGbfsField> QuestionTemplateGbfsFields =>
			Set<QuestionTemplateGbfsField>();

		public DbSet<GameAttempt> GameAttempts => Set<GameAttempt>();

		public DbSet<GameAttemptQuestion> GameAttemptQuestions =>
			Set<GameAttemptQuestion>();

		public DbSet<GameAttemptQuestionOption> GameAttemptQuestionOptions =>
			Set<GameAttemptQuestionOption>();

		public DbSet<ProviderFeed> ProviderFeeds => Set<ProviderFeed>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(
				typeof(ApplicationDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
