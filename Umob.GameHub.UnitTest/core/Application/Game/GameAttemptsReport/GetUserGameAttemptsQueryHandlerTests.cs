using System.Reflection;
using Moq;
using NUnit.Framework;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.GameAttemptsReport;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Game.GameAttemptsReport;

[TestFixture]
public sealed class GetUserGameAttemptsQueryHandlerTests
{
	[Test]
	public async Task Handle_WhenUserHasAttempts_ShouldReturnAttemptsReport()
	{
		// Arrange
		const long userId = 1;

		var attempt1 = GameAttempt.Create(userId, 60);
		SetPrivateProperty(attempt1, nameof(GameAttempt.Id), 10L);
		SetPrivateProperty(attempt1, nameof(GameAttempt.Score), 50);
		SetPrivateProperty(attempt1, nameof(GameAttempt.IsWon), true);
		SetPrivateProperty(attempt1, nameof(GameAttempt.StartedOn), new DateTime(2026, 5, 28, 10, 0, 0));
		SetPrivateProperty(attempt1, nameof(GameAttempt.EndedOn), new DateTime(2026, 5, 28, 10, 1, 0));

		var attempt2 = GameAttempt.Create(userId, 60);
		SetPrivateProperty(attempt2, nameof(GameAttempt.Id), 11L);
		SetPrivateProperty(attempt2, nameof(GameAttempt.Score), -20);
		SetPrivateProperty(attempt2, nameof(GameAttempt.IsWon), false);
		SetPrivateProperty(attempt2, nameof(GameAttempt.StartedOn), new DateTime(2026, 5, 28, 11, 0, 0));
		SetPrivateProperty(attempt2, nameof(GameAttempt.EndedOn), new DateTime(2026, 5, 28, 11, 1, 0));

		var attempts = new List<GameAttempt>
		{
			attempt1,
			attempt2
		};

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(attempts);

		var handler = new GetUserGameAttemptsQueryHandler(
			gameAttemptRepository.Object);

		var query = new GetUserGameAttemptsQuery(userId);

		// Act
		var response = await handler.Handle(
			query,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response.Count, Is.EqualTo(2));

		Assert.That(response[0].AttemptId, Is.EqualTo(10));
		Assert.That(response[0].Score, Is.EqualTo(50));
		Assert.That(response[0].IsWon, Is.True);
		Assert.That(response[0].StartedOn, Is.EqualTo(new DateTime(2026, 5, 28, 10, 0, 0)));
		Assert.That(response[0].EndedOn, Is.EqualTo(new DateTime(2026, 5, 28, 10, 1, 0)));
		Assert.That(response[0].DurationSeconds, Is.EqualTo(60));

		Assert.That(response[1].AttemptId, Is.EqualTo(11));
		Assert.That(response[1].Score, Is.EqualTo(-20));
		Assert.That(response[1].IsWon, Is.False);
		Assert.That(response[1].StartedOn, Is.EqualTo(new DateTime(2026, 5, 28, 11, 0, 0)));
		Assert.That(response[1].EndedOn, Is.EqualTo(new DateTime(2026, 5, 28, 11, 1, 0)));
		Assert.That(response[1].DurationSeconds, Is.EqualTo(60));

		gameAttemptRepository.Verify(
			x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task Handle_WhenUserHasNoAttempts_ShouldReturnEmptyList()
	{
		// Arrange
		const long userId = 1;

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<GameAttempt>());

		var handler = new GetUserGameAttemptsQueryHandler(
			gameAttemptRepository.Object);

		var query = new GetUserGameAttemptsQuery(userId);

		// Act
		var response = await handler.Handle(
			query,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response, Is.Empty);

		gameAttemptRepository.Verify(
			x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Test]
	public async Task Handle_ShouldCallRepositoryWithRequestUserId()
	{
		// Arrange
		const long userId = 7;

		var gameAttemptRepository = new Mock<IGameAttemptRepository>();

		gameAttemptRepository
			.Setup(x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<GameAttempt>());

		var handler = new GetUserGameAttemptsQueryHandler(
			gameAttemptRepository.Object);

		var query = new GetUserGameAttemptsQuery(userId);

		// Act
		await handler.Handle(
			query,
			CancellationToken.None);

		// Assert
		gameAttemptRepository.Verify(
			x => x.GetByUserIdAsync(
				userId,
				It.IsAny<CancellationToken>()),
			Times.Once);
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