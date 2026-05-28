using System.Reflection;
using Moq;
using NUnit.Framework;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.AnswerQuestion;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Game.AnswerQuestion;

[TestFixture]
public sealed class AnswerQuestionCommandHandlerTests
{
	[Test]
	public async Task Handle_WhenSelectedOptionIsCorrect_ShouldAdd50PointsAndMarkQuestionAsAnswered()
	{
		// Arrange
		var gameAttempt = GameAttempt.Create(1, 60);
		SetPrivateProperty(gameAttempt, nameof(GameAttempt.Id), 10L);

		var question = GameAttemptQuestion.Create(1);
		SetPrivateProperty(question, nameof(GameAttemptQuestion.Id), 100L);

		var correctOption = GameAttemptQuestionOption.Create(
			"A",
			"10",
			true);

		SetPrivateProperty(correctOption, nameof(GameAttemptQuestionOption.Id), 200L);

		var wrongOption = GameAttemptQuestionOption.Create(
			"B",
			"5",
			false);

		SetPrivateProperty(wrongOption, nameof(GameAttemptQuestionOption.Id), 201L);

		question.Options.Add(correctOption);
		question.Options.Add(wrongOption);

		gameAttempt.Questions.Add(question);

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetForAnswerAsync(
				10,
				1,
				100,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(gameAttempt);

		gameAttemptRepository
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var handler = new AnswerQuestionCommandHandler(
			gameAttemptRepository.Object);

		var command = new AnswerQuestionCommand(
			AttemptId: 10,
			UserId: 1,
			QuestionId: 100,
			SelectedOptionId: 200);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response!.AttemptId, Is.EqualTo(10));
		Assert.That(response.QuestionId, Is.EqualTo(100));
		Assert.That(response.SelectedOptionId, Is.EqualTo(200));
		Assert.That(response.IsCorrect, Is.True);
		Assert.That(response.Score, Is.EqualTo(50));

		Assert.That(gameAttempt.Score, Is.EqualTo(50));
		Assert.That(question.IsAnswered, Is.True);

		gameAttemptRepository.Verify(
			x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task Handle_WhenSelectedOptionIsWrong_ShouldSubtract20PointsAndMarkQuestionAsAnswered()
	{
		// Arrange
		var gameAttempt = GameAttempt.Create(1, 60);
		SetPrivateProperty(gameAttempt, nameof(GameAttempt.Id), 10L);

		var question = GameAttemptQuestion.Create(1);
		SetPrivateProperty(question, nameof(GameAttemptQuestion.Id), 100L);

		var wrongOption = GameAttemptQuestionOption.Create(
			"A",
			"5",
			false);

		SetPrivateProperty(wrongOption, nameof(GameAttemptQuestionOption.Id), 200L);

		question.Options.Add(wrongOption);
		gameAttempt.Questions.Add(question);

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetForAnswerAsync(
				10,
				1,
				100,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(gameAttempt);

		gameAttemptRepository
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var handler = new AnswerQuestionCommandHandler(
			gameAttemptRepository.Object);

		var command = new AnswerQuestionCommand(
			AttemptId: 10,
			UserId: 1,
			QuestionId: 100,
			SelectedOptionId: 200);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response!.IsCorrect, Is.False);
		Assert.That(response.Score, Is.EqualTo(-20));

		Assert.That(gameAttempt.Score, Is.EqualTo(-20));
		Assert.That(question.IsAnswered, Is.True);

		gameAttemptRepository.Verify(
			x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task Handle_WhenGameAttemptDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetForAnswerAsync(
				10,
				1,
				100,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync((GameAttempt?)null);

		var handler = new AnswerQuestionCommandHandler(
			gameAttemptRepository.Object);

		var command = new AnswerQuestionCommand(
			AttemptId: 10,
			UserId: 1,
			QuestionId: 100,
			SelectedOptionId: 200);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Null);

		gameAttemptRepository.Verify(
			x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task Handle_WhenQuestionIsAlreadyAnswered_ShouldReturnNull()
	{
		// Arrange
		var gameAttempt = GameAttempt.Create(1, 60);
		SetPrivateProperty(gameAttempt, nameof(GameAttempt.Id), 10L);

		var question = GameAttemptQuestion.Create(1);
		SetPrivateProperty(question, nameof(GameAttemptQuestion.Id), 100L);

		question.MarkAsAnswered();

		var option = GameAttemptQuestionOption.Create(
			"A",
			"10",
			true);

		SetPrivateProperty(option, nameof(GameAttemptQuestionOption.Id), 200L);

		question.Options.Add(option);
		gameAttempt.Questions.Add(question);

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetForAnswerAsync(
				10,
				1,
				100,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(gameAttempt);

		var handler = new AnswerQuestionCommandHandler(
			gameAttemptRepository.Object);

		var command = new AnswerQuestionCommand(
			AttemptId: 10,
			UserId: 1,
			QuestionId: 100,
			SelectedOptionId: 200);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Null);

		gameAttemptRepository.Verify(
			x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Test]
	public async Task Handle_WhenSelectedOptionDoesNotExist_ShouldReturnNull()
	{
		// Arrange
		var gameAttempt = GameAttempt.Create(1, 60);
		SetPrivateProperty(gameAttempt, nameof(GameAttempt.Id), 10L);

		var question = GameAttemptQuestion.Create(1);
		SetPrivateProperty(question, nameof(GameAttemptQuestion.Id), 100L);

		var existingOption = GameAttemptQuestionOption.Create(
			"A",
			"10",
			true);

		SetPrivateProperty(existingOption, nameof(GameAttemptQuestionOption.Id), 200L);

		question.Options.Add(existingOption);
		gameAttempt.Questions.Add(question);

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetForAnswerAsync(
				10,
				1,
				100,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(gameAttempt);

		var handler = new AnswerQuestionCommandHandler(
			gameAttemptRepository.Object);

		var command = new AnswerQuestionCommand(
			AttemptId: 10,
			UserId: 1,
			QuestionId: 100,
			SelectedOptionId: 999);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Null);
		Assert.That(gameAttempt.Score, Is.EqualTo(0));
		Assert.That(question.IsAnswered, Is.False);

		gameAttemptRepository.Verify(
			x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
			Times.Never);
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