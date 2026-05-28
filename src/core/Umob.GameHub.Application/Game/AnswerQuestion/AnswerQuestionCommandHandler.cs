using MediatR;
using Umob.GameHub.Application.Abstractions.StartGame;
using Umob.GameHub.Application.Game.AnswerQuestion.Dtos;

namespace Umob.GameHub.Application.Game.AnswerQuestion
{
	public sealed class AnswerQuestionCommandHandler(IGameAttemptRepository gameAttemptRepository)
		: IRequestHandler<AnswerQuestionCommand, AnswerQuestionResponse?>
	{
		public async Task<AnswerQuestionResponse?> Handle(
			AnswerQuestionCommand request,
			CancellationToken cancellationToken)
		{
			var gameAttempt = await gameAttemptRepository.GetForAnswerAsync(
				request.AttemptId,
				request.UserId,
				request.QuestionId,
				cancellationToken);

			if (gameAttempt is null)
			{
				return null;
			}

			var question = gameAttempt.Questions.FirstOrDefault();

			if (question is null || question.IsAnswered)
			{
				return null;
			}

			var selectedOption = question.Options.FirstOrDefault(
				option => option.Id == request.SelectedOptionId);

			if (selectedOption is null)
			{
				return null;
			}

			var isCorrect = selectedOption.IsCorrect;

			gameAttempt.ApplyAnswer(isCorrect);

			question.MarkAsAnswered();

			await gameAttemptRepository.SaveChangesAsync(cancellationToken);

			return new AnswerQuestionResponse(
				gameAttempt.Id,
				question.Id,
				selectedOption.Id,
				isCorrect,
				gameAttempt.Score);
		}
	}
}
