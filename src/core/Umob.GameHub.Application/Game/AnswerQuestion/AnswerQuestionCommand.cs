using MediatR;
using Umob.GameHub.Application.Game.AnswerQuestion.Dtos;

namespace Umob.GameHub.Application.Game.AnswerQuestion
{
	public sealed record AnswerQuestionCommand(
		long AttemptId,
		long UserId,
		long QuestionId,
		long SelectedOptionId) 
		: IRequest<AnswerQuestionResponse?>;
}
