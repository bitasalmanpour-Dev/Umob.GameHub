namespace Umob.GameHub.Domain.Entities
{
	public sealed class GameAttempt
	{
		public long Id { get; private set; }

		public long UserId { get; private set; }

		public int Score { get; private set; }

		public bool? IsWon { get; private set; }

		public DateTime StartedOn { get; private set; }

		public DateTime? EndedOn { get; private set; }

		public int DurationSeconds { get; private set; }

		public ICollection<GameAttemptQuestion> Questions { get; private set; } =
			new List<GameAttemptQuestion>();

		public static GameAttempt Create(long userId, int durationSeconds)
		{
			return new GameAttempt
			{
				UserId = userId,
				Score = 0,
				IsWon = null,
				StartedOn = DateTime.UtcNow,
				EndedOn = null,
				DurationSeconds = durationSeconds
			};
		}

		public void ApplyAnswer(bool isCorrect)
		{
			Score += isCorrect ? 50 : -20;
		}

		private GameAttempt()
		{
		}
	}
}
