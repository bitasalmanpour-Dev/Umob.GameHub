using Umob.GameHub.Application.Game.GameStrategy;

namespace Umob.GameHub.Application.Game.GameStrategeis
{
	public interface IQuestionAnswerStrategyResolver
	{
		IQuestionAnswerStrategy Resolve(string calculationType);
	}

	public sealed class QuestionAnswerStrategyResolver(
		IEnumerable<IQuestionAnswerStrategy> strategies)
		: IQuestionAnswerStrategyResolver
	{
		public IQuestionAnswerStrategy Resolve(string calculationType)
		{
			var strategy = strategies.FirstOrDefault(x =>
				string.Equals(
					x.StrategyType,
					calculationType,
					StringComparison.OrdinalIgnoreCase));

			if (strategy is null)
			{
				throw new InvalidOperationException(
					$"Calculation strategy '{calculationType}' was not found.");
			}

			return strategy;
		}
	}
}
