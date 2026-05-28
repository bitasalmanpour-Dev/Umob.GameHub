namespace Umob.GameHub.Domain.Entities
{
	public sealed class QuestionTemplateGbfsField
	{
		public long Id { get; private set; }

		public long QuestionTemplateId { get; private set; }

		public string FeedName { get; private set; } = string.Empty;

		public string CollectionPath { get; private set; } = string.Empty;

		public string FieldName { get; private set; } = string.Empty;

		public long ProviderFeedsId { get; private set; }

		public DateTime CreatedOn { get; private set; }

		public QuestionTemplate QuestionTemplate { get; private set; } = null!;

		public ProviderFeed ProviderFeed { get; private set; } = null!;

        public QuestionTemplateGbfsField()
        {
            
        }

    }
}
