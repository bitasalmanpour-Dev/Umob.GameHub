namespace Umob.GameHub.Domain.Entities
{
	public sealed class QuestionTemplate
	{
		public long Id { get; private set; }

		public string TextTemplate { get; private set; }

		public bool IsActive { get; private set; }

		public DateTime CreatedOn { get; private set; }

		public string StrategyType { get; private set; } = string.Empty;

		public ICollection<QuestionTemplateGbfsField> GbfsFields { get; private set; } =
			new List<QuestionTemplateGbfsField>();

        public QuestionTemplate()
        {
            
        }
    }
}
