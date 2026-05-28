using System.Reflection;
using Moq;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Game.GameStrategeis;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Game.GameStrategeis;

[TestFixture]
public sealed class MaxValueQuestionAnswerStrategyTests
{
	[Test]
	public void StrategyType_ShouldReturnMaxValue()
	{
		// Arrange
		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new MaxValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var strategyType = strategy.StrategyType;

		// Assert
		Assert.That(strategyType, Is.EqualTo("MaxValue"));
	}

	[Test]
	public async Task CalculateAsync_WhenFieldsHaveNumericValues_ShouldReturnMaximumValue()
	{
		// Arrange
		const string firstUrl = "https://example.com/provider-1/station_status.json";
		const string secondUrl = "https://example.com/provider-2/station_status.json";

		var firstJson = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 8
                    },
                    {
                        "station_id": "2",
                        "num_vehicles_available": 14
                    }
                ]
            }
        }
        """;

		var secondJson = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "3",
                        "num_vehicles_available": 21
                    },
                    {
                        "station_id": "4",
                        "num_vehicles_available": 5
                    }
                ]
            }
        }
        """;

		var fields = new List<QuestionTemplateGbfsField>
		{
			CreateQuestionTemplateGbfsField(firstUrl),
			CreateQuestionTemplateGbfsField(secondUrl)
		};

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				firstUrl,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(firstJson);

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				secondUrl,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(secondJson);

		var strategy = new MaxValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			fields,
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.EqualTo("21"));

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				firstUrl,
				It.IsAny<CancellationToken>()),
			Times.Once);

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				secondUrl,
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task CalculateAsync_WhenFieldHasEmptyUrl_ShouldSkipThatField()
	{
		// Arrange
		const string validUrl = "https://example.com/provider-1/station_status.json";

		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "1",
                        "num_vehicles_available": 12
                    }
                ]
            }
        }
        """;

		var fields = new List<QuestionTemplateGbfsField>
		{
			CreateQuestionTemplateGbfsField(validUrl),
			CreateQuestionTemplateGbfsField("")
		};

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				validUrl,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new MaxValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			fields,
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.EqualTo("12"));

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				validUrl,
				It.IsAny<CancellationToken>()),
			Times.Once);

		gbfsCaller.Verify(
			x => x.GetJsonAsync(
				"",
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task CalculateAsync_WhenProviderFeedIsNull_ShouldSkipFieldAndReturnNull()
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

		var strategy = new MaxValueQuestionAnswerStrategy(
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
	public async Task CalculateAsync_WhenFieldsAreEmpty_ShouldReturnNull()
	{
		// Arrange
		var gbfsCaller = new Mock<IGbfsCaller>();

		var strategy = new MaxValueQuestionAnswerStrategy(
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
	public async Task CalculateAsync_WhenCollectionPathDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		const string url = "https://example.com/provider-1/station_status.json";

		var json = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": 1,
                        "num_vehicles_available": 18
                    }
                ]
            }
        }
        """;

		var field = CreateQuestionTemplateGbfsField(
			providerFeedUrl: url,
			collectionPath: "data.invalid",
			fieldName: "num_vehicles_available");

		var gbfsCaller = new Mock<IGbfsCaller>();

		gbfsCaller
			.Setup(x => x.GetJsonAsync(
				url,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(json);

		var strategy = new MaxValueQuestionAnswerStrategy(
			gbfsCaller.Object);

		// Act
		var result = await strategy.CalculateAsync(
			new List<QuestionTemplateGbfsField> { field },
			CancellationToken.None);

		// Assert
		Assert.That(result, Is.Null);
	}

	private static QuestionTemplateGbfsField CreateQuestionTemplateGbfsField(
		string providerFeedUrl,
		string collectionPath = "data.stations",
		string fieldName = "num_vehicles_available")
	{
		var field = CreateInstance<QuestionTemplateGbfsField>();

		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.Id), 1L);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.QuestionTemplateId), 1L);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.FeedName), "station_status");
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.CollectionPath), collectionPath);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.FieldName), fieldName);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.ProviderFeedsId), 1L);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.CreatedOn), DateTime.UtcNow);

		var providerFeed = CreateInstance<ProviderFeed>();

		SetPrivateProperty(providerFeed, nameof(ProviderFeed.Id), 1L);
		SetPrivateProperty(providerFeed, nameof(ProviderFeed.FeedName), "station_status");
		SetPrivateProperty(providerFeed, nameof(ProviderFeed.Url), providerFeedUrl);
		SetPrivateProperty(providerFeed, nameof(ProviderFeed.ProviderId), 1L);

		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.ProviderFeed), providerFeed);

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