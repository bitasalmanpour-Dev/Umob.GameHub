using System.Reflection;
using Moq;
using NUnit.Framework;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Game.GameStrategeis;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Game.GameStrategies;

[TestFixture]
public sealed class DirectValueQuestionAnswerStrategyTests
{
	[Test]
	public void StrategyType_ShouldBeDirectValue()
	{
		// Arrange
		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act & Assert
		Assert.That(strategy.StrategyType, Is.EqualTo("DirectValue"));
	}

	[Test]
	public async Task CalculateAsync_WhenFieldExists_ShouldReadValueFromGbfsJson()
	{
		// Arrange
		const string url = "https://example.com/station_status.json";

		var json = """
        {
            "last_updated": "2026-05-28T10:00:00Z",
            "ttl": 30,
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		var field = CreateGbfsField(
			collectionPath: "data.stations",
			fieldName: "num_vehicles_available",
			providerFeedUrl: url);

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.EqualTo("18"));

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task CalculateAsync_WhenJsonFieldIsString_ShouldReturnStringValue()
	{
		// Arrange
		const string url = "https://example.com/station_information.json";

		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "name": "Dubai Center"
                    }
                ]
            }
        }
        """;

		var field = CreateGbfsField(
			collectionPath: "data.stations",
			fieldName: "name",
			providerFeedUrl: url);

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.EqualTo("Dubai Center"));
	}

	[Test]
	public async Task CalculateAsync_WhenFieldsAreEmpty_ShouldReturnNull()
	{
		// Arrange
		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField>(),
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task CalculateAsync_WhenProviderFeedIsNull_ShouldReturnNull()
	{
		// Arrange
		var field = CreateInstance<QuestionTemplateGbfsField>();

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.CollectionPath),
			"data.stations");

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.FieldName),
			"num_vehicles_available");

		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task CalculateAsync_WhenProviderFeedUrlIsEmpty_ShouldReturnNull()
	{
		// Arrange
		var field = CreateGbfsField(
			collectionPath: "data.stations",
			fieldName: "num_vehicles_available",
			providerFeedUrl: "");

		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task CalculateAsync_WhenCollectionPathDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		const string url = "https://example.com/station_status.json";

		var json = """
        {
            "data": {
                "stations": [
                    {
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		var field = CreateGbfsField(
			collectionPath: "data.invalid",
			fieldName: "num_vehicles_available",
			providerFeedUrl: url);

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public async Task CalculateAsync_WhenFieldNameDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		const string url = "https://example.com/station_status.json";

		var json = """
        {
            "data": {
                "stations": [
                    {
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		var field = CreateGbfsField(
			collectionPath: "data.stations",
			fieldName: "wrong_field",
			providerFeedUrl: url);

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new DirectValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);
	}

	private static QuestionTemplateGbfsField CreateGbfsField(
		string collectionPath,
		string fieldName,
		string providerFeedUrl)
	{
		var field = CreateInstance<QuestionTemplateGbfsField>();

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.Id),
			1L);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.QuestionTemplateId),
			1L);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.CollectionPath),
			collectionPath);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.FieldName),
			fieldName);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.FeedName),
			"station_status");

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.ProviderFeedsId),
			1L);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.CreatedOn),
			DateTime.UtcNow);

		var providerFeed = CreateInstance<ProviderFeed>();

		SetPrivateProperty(
			providerFeed,
			nameof(ProviderFeed.Id),
			1L);

		SetPrivateProperty(
			providerFeed,
			nameof(ProviderFeed.FeedName),
			"station_status");

		SetPrivateProperty(
			providerFeed,
			nameof(ProviderFeed.Url),
			providerFeedUrl);

		SetPrivateProperty(
			providerFeed,
			nameof(ProviderFeed.ProviderId),
			1L);

		SetPrivateProperty(
			field,
			nameof(QuestionTemplateGbfsField.ProviderFeed),
			providerFeed);

		return field;
	}

	private static T CreateInstance<T>()
	{
		var instance = Activator.CreateInstance(
			typeof(T),
			nonPublic: true);

		if (instance is null)
		{
			throw new InvalidOperationException(
				$"Could not create instance of type '{typeof(T).Name}'.");
		}

		return (T)instance;
	}

	private static void SetPrivateProperty<T>(
		T instance,
		string propertyName,
		object? value)
	{
		var property = typeof(T).GetProperty(
			propertyName,
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		if (property is null)
		{
			throw new InvalidOperationException(
				$"Property '{propertyName}' was not found on type '{typeof(T).Name}'.");
		}

		property.SetValue(instance, value);
	}
}