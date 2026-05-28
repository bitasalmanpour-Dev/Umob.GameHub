using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.StartGame
{
	public interface IQuestionTemplateRepository
    {
        Task<IReadOnlyList<QuestionTemplate>> GetActiveTemplatesWithFieldsAsync(
            CancellationToken cancellationToken = default);
    }
}
