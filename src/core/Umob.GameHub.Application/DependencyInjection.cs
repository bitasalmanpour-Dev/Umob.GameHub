using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umob.GameHub.Application.Common;
using Umob.GameHub.Application.Game.GameStrategeis;
using Umob.GameHub.Application.Game.GameStrategy;

namespace Umob.GameHub.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			var assembly = typeof(DependencyInjection).Assembly;

			services.AddMediatR(configuration =>
			{
				configuration.RegisterServicesFromAssembly(assembly);
				configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
			});

			services.AddValidatorsFromAssembly(assembly);

			services.AddScoped<IQuestionAnswerStrategy, DirectValueQuestionAnswerStrategy>();
			services.AddScoped<IQuestionAnswerStrategy, MaxValueQuestionAnswerStrategy>();
			services.AddScoped<IQuestionAnswerStrategyResolver, QuestionAnswerStrategyResolver>();
			return services;
		}
	}
}
