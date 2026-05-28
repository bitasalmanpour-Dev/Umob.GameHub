using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Game.GameStrategy;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Game.GameStrategeis
{
	public sealed class MaxValueQuestionAnswerStrategy(IGbfsCaller gbfsFeed)
		: IQuestionAnswerStrategy
	{
		private const string strategyType = "MaxValue";
	
		public string StrategyType => strategyType;

		public async Task<string?> CalculateAsync(
			IReadOnlyCollection<QuestionTemplateGbfsField> fields,
			CancellationToken cancellationToken = default)
		{
			var allValues = new List<decimal>();

			foreach (var field in fields)
			{
				if (field.ProviderFeed is null ||
					string.IsNullOrWhiteSpace(field.ProviderFeed.Url))
				{
					continue;
				}

				var json = await gbfsFeed.GetJsonAsync(
					field.ProviderFeed.Url,
					cancellationToken);
				
				var values = GbfsJsonValueReader.ReadNumericValues(
					json,
					field.CollectionPath,
					field.FieldName);

				allValues.AddRange(values);
			}

			if (allValues.Count == 0)
			{
				return null;
			}

			var max = allValues.Max();

			return max.ToString("0");
		}
	}
}
