using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Game.GameStrategy
{
	public interface IQuestionAnswerStrategy
	{
		string StrategyType { get; }

		Task<string?> CalculateAsync(
			IReadOnlyCollection<QuestionTemplateGbfsField> fields,
			CancellationToken cancellationToken = default);
	}
}
