using MediatR;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.GameStrategeis;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Game.StartGame
{
	public sealed class StartGameCommandHandler(
		IQuestionTemplateRepository questionTemplateRepository,
			IGameAttemptRepository gameAttemptRepository,
			IQuestionAnswerStrategyResolver strategyResolver)
		: IRequestHandler<StartGameCommand, StartGameResponse?>
	{
		
		private const int GameDurationSeconds = 60;

		public async Task<StartGameResponse?> Handle(
			StartGameCommand request,
			CancellationToken cancellationToken)
		{
			var questions =
				await questionTemplateRepository.GetActiveTemplatesWithFieldsAsync(
					cancellationToken);

			if (questions.Count == 0)
			{
				return null;
			}

			var gameAttempt = GameAttempt.Create(
				request.UserId,
				GameDurationSeconds);

			foreach (var template in questions)
			{
				if (template.GbfsFields.Count == 0)
				{
					throw new InvalidOperationException(
							   $"QuestionTemplate {template.Id} has no GBFS fields.");
				}

				var strategy = strategyResolver.Resolve(
					template.StrategyType);

				var correctAnswer = await strategy.CalculateAsync(
					template.GbfsFields.ToList(),
					cancellationToken);

				if (string.IsNullOrWhiteSpace(correctAnswer))
				{
					continue;
				}

				var gameAttemptQuestion = GameAttemptQuestion.Create(
					template.Id);

				var options = CreateOptions(correctAnswer);

				foreach (var option in options)
				{
					gameAttemptQuestion.Options.Add(
						GameAttemptQuestionOption.Create(
							option.OptionKey,
							option.OptionValue,
							option.IsCorrect));
				}

				gameAttempt.Questions.Add(gameAttemptQuestion);
			}

			if (gameAttempt.Questions.Count == 0)
			{
				return null;
			}

			await gameAttemptRepository.AddAsync(
				gameAttempt,
				cancellationToken);

			var responseQuestions = gameAttempt.Questions
				.Select(question =>
				{
					var template = questions.First(x => x.Id == question.QuestionsTemplatesId);

					return new StartGameQuestionResponse(
						question.Id,
						question.QuestionsTemplatesId,
						template.TextTemplate,
						question.Options
							.OrderBy(option => option.OptionKey)
							.Select(option => new StartGameQuestionOptionResponse(
								option.Id,
								option.OptionKey,
								option.OptionValue))
							.ToList());
				})
				.ToList();

			return new StartGameResponse(
				gameAttempt.Id,
				gameAttempt.Score,
				gameAttempt.DurationSeconds,
				gameAttempt.StartedOn,
				responseQuestions);


		}

		private static IReadOnlyList<OptionItem> CreateOptions(string correctAnswer)
		{
			var optionKeys = new[] { "A", "B", "C" };

			if (int.TryParse(correctAnswer, out var number))
			{
				var values = new HashSet<int>
				{
					number,
					number + 1,
					number + 2,
				};

				return values
					.OrderBy(_ => Guid.NewGuid())
					.Take(3)
					.Select((value, index) => new OptionItem(
						optionKeys[index],
						value.ToString(),
						value == number))
					.ToList();
			}

			var textValues = new List<string>
			{
				correctAnswer,
				"Unknown",
				"Not available",
				"0"
			};

			return textValues
				.Distinct()
				.OrderBy(_ => Guid.NewGuid())
				.Select((value, index) => new OptionItem(
					optionKeys[index],
					value,
					value == correctAnswer))
				.ToList();
		}

		private sealed record OptionItem(
			string OptionKey,
			string OptionValue,
			bool IsCorrect);
	}
}
