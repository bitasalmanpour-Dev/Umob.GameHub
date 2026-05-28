using System.Text.Json;

namespace Umob.GameHub.Application.Game.GameStrategeis
{
	public static class GbfsJsonValueReader
	{
		public static string? ReadFirstValue(
			string json,
			string collectionPath,
			string fieldName)
		{
			using var document = JsonDocument.Parse(json);

			if (!TryGetElementByPath(document.RootElement, collectionPath, out var element))
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

		public static IReadOnlyList<decimal> ReadNumericValues(
			string json,
			string collectionPath,
			string fieldName)
		{
			using var document = JsonDocument.Parse(json);

			if (!TryGetElementByPath(document.RootElement, collectionPath, out var element))
			{
				return [];
			}

			var values = new List<decimal>();

			if (element.ValueKind == JsonValueKind.Array)
			{
				foreach (var item in element.EnumerateArray())
				{
					if (item.ValueKind != JsonValueKind.Object)
					{
						continue;
					}

					if (!item.TryGetProperty(fieldName, out var fieldElement))
					{
						continue;
					}

					if (fieldElement.TryGetDecimal(out var value))
					{
						values.Add(value);
					}
				}

				return values;
			}

			if (element.ValueKind == JsonValueKind.Object &&
				element.TryGetProperty(fieldName, out var singleField) &&
				singleField.TryGetDecimal(out var singleValue))
			{
				values.Add(singleValue);
			}

			return values;
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
				if (result.ValueKind != JsonValueKind.Object
					|| !result.TryGetProperty(part, out result))
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
