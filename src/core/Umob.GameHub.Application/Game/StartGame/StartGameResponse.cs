namespace Umob.GameHub.Application.Game.StartGame
{
	public sealed record StartGameResponse(
        long AttemptId,
        int Score,
        int DurationSeconds,
        DateTime StartedOn,
        IReadOnlyCollection<StartGameQuestionResponse> Questions);

    public sealed record StartGameQuestionResponse(
        long QuestionId,
        long QuestionTemplateId,
        string QuestionText,
        IReadOnlyCollection<StartGameQuestionOptionResponse> Options);

    public sealed record StartGameQuestionOptionResponse(
        long OptionId,
        string OptionKey,
        string OptionValue);
}
