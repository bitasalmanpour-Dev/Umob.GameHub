namespace Umob.GameHub.Api.V1.Controllers.Dtos
{
	public sealed record AnswerQuestionRequest(
		long UserId,
		long AttemptId,
		long QuestionId,
		long SelectedOptionId);
}
