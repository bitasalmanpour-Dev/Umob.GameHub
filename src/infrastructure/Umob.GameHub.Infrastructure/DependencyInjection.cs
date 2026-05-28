using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Infrastructure.Authentication;
using Umob.GameHub.Infrastructure.Persistence;
using Umob.GameHub.Infrastructure.Repositories;
using Umob.GameHub.Infrastructure.Services;

namespace Umob.GameHub.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services
				.AddHttpClients()
				.AddPersistence(configuration)
				.AddOptions(configuration)
				.AddRepositories()
				.AddExternalServices();

			return services;
		}

		private static IServiceCollection AddHttpClients(
			this IServiceCollection services)
		{
			services.AddHttpClient("GbfsClient", client =>
			{
				client.Timeout = TimeSpan.FromSeconds(2);
			});

			return services;
		}

		private static IServiceCollection AddPersistence(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(
					configuration.GetConnectionString("DefaultConnection"));
			});

			return services;
		}

		private static IServiceCollection AddOptions(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services.Configure<JwtOptions>(
				configuration.GetSection(JwtOptions.SectionName));

			return services;
		}

		private static IServiceCollection AddRepositories(
			this IServiceCollection services)
		{
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IGbfsProviderRepository, GbfsProviderRepository>();
			services.AddScoped<IQuestionTemplateGbfsFieldRepository, QuestionTemplateGbfsFieldRepository>();
			services.AddScoped<IQuestionTemplateRepository, QuestionTemplateRepository>();
			services.AddScoped<IGameAttemptRepository, GameAttemptRepository>();
			services.AddScoped<IProviderFeedRepository, ProviderFeedRepository>();

			return services;
		}

		private static IServiceCollection AddExternalServices(
			this IServiceCollection services)
		{
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.AddScoped<IJwtToken, JwtToken>();
			services.AddScoped<IGbfsCaller, GbfsCaller>();

			return services;
		}
	}
}
