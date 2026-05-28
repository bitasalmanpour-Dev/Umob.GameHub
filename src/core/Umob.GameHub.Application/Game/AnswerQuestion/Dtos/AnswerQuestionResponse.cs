namespace Umob.GameHub.Application.Game.AnswerQuestion.Dtos
{
	public sealed record AnswerQuestionResponse(
		long AttemptId,
		long QuestionId,
		long SelectedOptionId,
		bool IsCorrect,
		int Score);
}
