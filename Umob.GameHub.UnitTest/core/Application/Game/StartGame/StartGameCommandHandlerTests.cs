using System.Reflection;
using Moq;
using NUnit.Framework;
using Umob.GameHub.Application.Abstractions;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.GameStrategeis;
using Umob.GameHub.Application.Game.GameStrategy;
using Umob.GameHub.Application.Game.StartGame;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Game.StartGame;

[TestFixture]
public sealed class StartGameCommandHandlerTests
{
	[Test]
	public async Task Handle_WhenActiveQuestionsExist_ShouldCreateGameAttemptAndReturnQuestions()
	{
		// Arrange
		var template1 = CreateQuestionTemplate(
			id: 1,
			textTemplate: "What is the number of available bikes?",
			strategyType: "DirectValue");

		template1.GbfsFields.Add(CreateGbfsField(
			id: 1,
			questionTemplateId: 1));

		var template2 = CreateQuestionTemplate(
			id: 2,
			textTemplate: "What is the max available bike?",
			strategyType: "MaxValue");

		template2.GbfsFields.Add(CreateGbfsField(
			id: 2,
			questionTemplateId: 2));

		var templates = new List<QuestionTemplate>
		{
			template1,
			template2
		};

		var questionTemplateRepository = new Mock<IQuestionTemplateRepository>();
		var gameAttemptRepository = new Mock<IGameAttemptRepository>();
		var strategyResolver = new Mock<IQuestionAnswerStrategyResolver>();
		var directValueStrategy = new Mock<IQuestionAnswerStrategy>();
		var maxValueStrategy = new Mock<IQuestionAnswerStrategy>();

		questionTemplateRepository
			.Setup(x => x.GetActiveTemplatesWithFieldsAsync(
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(templates);

		strategyResolver
			.Setup(x => x.Resolve("DirectValue"))
			.Returns(directValueStrategy.Object);

		strategyResolver
			.Setup(x => x.Resolve("MaxValue"))
			.Returns(maxValueStrategy.Object);

		directValueStrategy
			.Setup(x => x.CalculateAsync(
				It.IsAny<IReadOnlyCollection<QuestionTemplateGbfsField>>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync("10");

		maxValueStrategy
			.Setup(x => x.CalculateAsync(
				It.IsAny<IReadOnlyCollection<QuestionTemplateGbfsField>>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync("20");

		GameAttempt? savedAttempt = null;

		gameAttemptRepository
			.Setup(x => x.AddAsync(
				It.IsAny<GameAttempt>(),
				It.IsAny<CancellationToken>()))
			.Callback<GameAttempt, CancellationToken>((attempt, _) =>
			{
				savedAttempt = attempt;

				SetPrivateProperty(attempt, nameof(GameAttempt.Id), 100L);

				var questionId = 1L;
				var optionId = 1L;

				foreach (var question in attempt.Questions)
				{
					SetPrivateProperty(question, nameof(GameAttemptQuestion.Id), questionId++);

					foreach (var option in question.Options)
					{
						SetPrivateProperty(option, nameof(GameAttemptQuestionOption.Id), optionId++);
					}
				}
			})
			.Returns(Task.CompletedTask);

		var handler = new StartGameCommandHandler(
			questionTemplateRepository.Object,
			gameAttemptRepository.Object,
			strategyResolver.Object);

		var command = new StartGameCommand(UserId: 5);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response!.AttemptId, Is.EqualTo(100));
		Assert.That(response.Score, Is.EqualTo(0));
		Assert.That(response.DurationSeconds, Is.EqualTo(60));
		Assert.That(response.Questions.Count, Is.EqualTo(2));

		Assert.That(response.Questions.First().QuestionTemplateId, Is.EqualTo(1));
		Assert.That(response.Questions.First().QuestionText, Is.EqualTo("What is the number of available bikes?"));
		Assert.That(response.Questions.First().Options.Count, Is.EqualTo(3));

		Assert.That(response.Questions.Last().QuestionTemplateId, Is.EqualTo(2));
		Assert.That(response.Questions.Last().QuestionText, Is.EqualTo("What is the max available bike?"));
		Assert.That(response.Questions.Last().Options.Count, Is.EqualTo(3));

		Assert.That(savedAttempt, Is.Not.Null);
		Assert.That(savedAttempt!.UserId, Is.EqualTo(5));
		Assert.That(savedAttempt.Questions.Count, Is.EqualTo(2));

		gameAttemptRepository.Verify(
			x => x.AddAsync(
				It.IsAny<GameAttempt>(),
				It.IsAny<CancellationToken>()),
			Times.Once);

		strategyResolver.Verify(x => x.Resolve("DirectValue"), Times.Once);
		strategyResolver.Verify(x => x.Resolve("MaxValue"), Times.Once);
	}

	[Test]
	public async Task Handle_WhenNoActiveQuestionsExist_ShouldReturnNull()
	{
		// Arrange
		var questionTemplateRepository = new Mock<IQuestionTemplateRepository>();
		var gameAttemptRepository = new Mock<IGameAttemptRepository>();
		var strategyResolver = new Mock<IQuestionAnswerStrategyResolver>();

		questionTemplateRepository
			.Setup(x => x.GetActiveTemplatesWithFieldsAsync(
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<QuestionTemplate>());

		var handler = new StartGameCommandHandler(
			questionTemplateRepository.Object,
			gameAttemptRepository.Object,
			strategyResolver.Object);

		var command = new StartGameCommand(UserId: 5);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Null);

		gameAttemptRepository.Verify(
			x => x.AddAsync(
				It.IsAny<GameAttempt>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public void Handle_WhenTemplateHasNoGbfsFields_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var template = CreateQuestionTemplate(
			id: 1,
			textTemplate: "Question without fields",
			strategyType: "DirectValue");

		var questionTemplateRepository = new Mock<IQuestionTemplateRepository>();
		var gameAttemptRepository = new Mock<IGameAttemptRepository>();
		var strategyResolver = new Mock<IQuestionAnswerStrategyResolver>();

		questionTemplateRepository
			.Setup(x => x.GetActiveTemplatesWithFieldsAsync(
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<QuestionTemplate> { template });

		var handler = new StartGameCommandHandler(
			questionTemplateRepository.Object,
			gameAttemptRepository.Object,
			strategyResolver.Object);

		var command = new StartGameCommand(UserId: 5);

		// Act & Assert
		var exception = Assert.ThrowsAsync<InvalidOperationException>(
			async () => await handler.Handle(
				command,
				CancellationToken.None));

		Assert.That(
			exception!.Message,
			Is.EqualTo("QuestionTemplate 1 has no GBFS fields."));

		gameAttemptRepository.Verify(
			x => x.AddAsync(
				It.IsAny<GameAttempt>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task Handle_WhenStrategyCannotCalculateAnswer_ShouldReturnNullAndNotSaveAttempt()
	{
		// Arrange
		var template = CreateQuestionTemplate(
			id: 1,
			textTemplate: "What is the number of available bikes?",
			strategyType: "DirectValue");

		template.GbfsFields.Add(CreateGbfsField(
			id: 1,
			questionTemplateId: 1));

		var questionTemplateRepository = new Mock<IQuestionTemplateRepository>();
		var gameAttemptRepository = new Mock<IGameAttemptRepository>();
		var strategyResolver = new Mock<IQuestionAnswerStrategyResolver>();
		var strategy = new Mock<IQuestionAnswerStrategy>();

		questionTemplateRepository
			.Setup(x => x.GetActiveTemplatesWithFieldsAsync(
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<QuestionTemplate> { template });

		strategyResolver
			.Setup(x => x.Resolve("DirectValue"))
			.Returns(strategy.Object);

		strategy
			.Setup(x => x.CalculateAsync(
				It.IsAny<IReadOnlyCollection<QuestionTemplateGbfsField>>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync((string?)null);

		var handler = new StartGameCommandHandler(
			questionTemplateRepository.Object,
			gameAttemptRepository.Object,
			strategyResolver.Object);

		var command = new StartGameCommand(UserId: 5);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Null);

		gameAttemptRepository.Verify(
			x => x.AddAsync(
				It.IsAny<GameAttempt>(),
				It.IsAny<CancellationToken>()),
			Times.Never);
	}

	private static QuestionTemplate CreateQuestionTemplate(
		long id,
		string textTemplate,
		string strategyType)
	{
		var template = CreateInstance<QuestionTemplate>();

		SetPrivateProperty(template, nameof(QuestionTemplate.Id), id);
		SetPrivateProperty(template, nameof(QuestionTemplate.TextTemplate), textTemplate);
		SetPrivateProperty(template, nameof(QuestionTemplate.StrategyType), strategyType);
		SetPrivateProperty(template, nameof(QuestionTemplate.IsActive), true);
		SetPrivateProperty(template, nameof(QuestionTemplate.CreatedOn), DateTime.UtcNow);

		return template;
	}

	private static QuestionTemplateGbfsField CreateGbfsField(
		long id,
		long questionTemplateId)
	{
		var field = CreateInstance<QuestionTemplateGbfsField>();

		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.Id), id);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.QuestionTemplateId), questionTemplateId);
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.FeedName), "station_status");
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.CollectionPath), "data.stations");
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.FieldName), "num_vehicles_available");
		SetPrivateProperty(field, nameof(QuestionTemplateGbfsField.CreatedOn), DateTime.UtcNow);

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