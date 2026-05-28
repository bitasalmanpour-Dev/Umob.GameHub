namespace Umob.GameHub.Domain.Entities
{
	public sealed class GameAttemptQuestion
	{
		public long Id { get; private set; }

		public long GameAttemptId { get; private set; }

		public long QuestionsTemplatesId { get; private set; }

		public bool IsAnswered { get; private set; }

		public DateTime CreatedOn { get; private set; }

		public GameAttempt GameAttempt { get; private set; } = null!;

		public ICollection<GameAttemptQuestionOption> Options { get; private set; } =
			new List<GameAttemptQuestionOption>();

		public static GameAttemptQuestion Create(long questionTemplateId)
		{
			return new GameAttemptQuestion
			{
				QuestionsTemplatesId = questionTemplateId,
				CreatedOn = DateTime.UtcNow,
				IsAnswered = false
			};
		}

		public void MarkAsAnswered()
		{
			IsAnswered = true;
		}

        public GameAttemptQuestion()
        {
            
        }
    }
}
