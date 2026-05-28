using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.StartGame
{
	public interface IQuestionTemplateGbfsFieldRepository
    {
        Task<QuestionTemplateGbfsField?> GetByQuestionTemplateIdAsync(
            long questionTemplateId,
            CancellationToken cancellationToken = default);
    }
}
