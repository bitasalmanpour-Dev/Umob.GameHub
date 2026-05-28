using System.Text.Json;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Game.GameStrategy;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Game.GameStrategeis
{
	public sealed class DirectValueQuestionAnswerStrategy(IGbfsCaller gbfsCaller) 
		: IQuestionAnswerStrategy
	{
		private const string strategyType = "DirectValue";

		public string StrategyType => strategyType;

		public async Task<string?> CalculateAsync(
			IReadOnlyCollection<QuestionTemplateGbfsField> fields,
			CancellationToken cancellationToken = default)
		{
			var field = fields.FirstOrDefault();

			if (field is null ||
				field.ProviderFeed is null ||
				string.IsNullOrWhiteSpace(field.ProviderFeed.Url))
			{
				return null;
			}

			var json = await gbfsCaller.GetJsonAsync(
				field.ProviderFeed.Url,
				cancellationToken);

			return ReadValueFromGbfsJson(
				json,
				field.CollectionPath,
				field.FieldName);
		}

		private static string? ReadValueFromGbfsJson(
			string json,
			string collectionPath,
			string fieldName)
		{
			using var document = JsonDocument.Parse(json);

			if (!TryGetElementByPath(
					document.RootElement,
					collectionPath,
					out var element))
			{
				return null;
			}

			if (element.ValueKind == JsonValueKind.Array)
			{
				element = element.EnumerateArray().FirstOrDefault();
			}

			if (element.ValueKind != JsonValueKind.Object 
				|| !element.TryGetProperty(fieldName, out var fieldElement))
			{
				return null;
			}

			return ConvertJsonValueToString(fieldElement);
		}

		private static bool TryGetElementByPath(
			JsonElement root,
			string path,
			out JsonElement result)
		{
			result = root;

			var parts = path.Split(
				'.',
				StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			foreach (var part in parts)
			{
				if (result.ValueKind != JsonValueKind.Object)
				{
					return false;
				}

				if (!result.TryGetProperty(part, out result))
				{
					return false;
				}
			}

			return true;
		}

		private static string ConvertJsonValueToString(JsonElement element)
		{
			return element.ValueKind switch
			{
				JsonValueKind.String => element.GetString() ?? string.Empty,
				JsonValueKind.Number => element.ToString(),
				JsonValueKind.True => "true",
				JsonValueKind.False => "false",
				JsonValueKind.Null => string.Empty,
				_ => element.ToString()
			};
		}
	}
}
