namespace Umob.GameHub.Domain.Entities
{
	public sealed class GameAttemptQuestionOption
	{
		public long Id { get; private set; }

		public long GameAttemptQuestionId { get; private set; }

		public string OptionKey { get; private set; } = string.Empty;

		public string OptionValue { get; private set; } = string.Empty;

		public bool IsCorrect { get; private set; }

		public DateTime CreatedOn { get; private set; }

		public GameAttemptQuestion GameAttemptQuestion { get; private set; } = null!;

		public static GameAttemptQuestionOption Create(
	    string optionKey,
	    string optionValue,
	    bool isCorrect)
		{
			return new GameAttemptQuestionOption
			{
				OptionKey = optionKey,
				OptionValue = optionValue,
				IsCorrect = isCorrect,
				CreatedOn = DateTime.UtcNow
			};
		}

        public GameAttemptQuestionOption()
        {
            
        }
    }
}
