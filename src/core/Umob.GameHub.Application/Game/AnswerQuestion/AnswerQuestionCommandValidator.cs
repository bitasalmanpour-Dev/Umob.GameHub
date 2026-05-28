using FluentValidation;

namespace Umob.GameHub.Application.Game.AnswerQuestion
{
	public sealed class AnswerQuestionCommandValidator
		: AbstractValidator<AnswerQuestionCommand>
	{
		public AnswerQuestionCommandValidator()
		{
			RuleFor(x => x.AttemptId)
				.GreaterThan(0)
				.WithMessage("AttemptId is required.");

			RuleFor(x => x.UserId)
				.GreaterThan(0)
				.WithMessage("UserId is required.");

			RuleFor(x => x.QuestionId)
				.GreaterThan(0)
				.WithMessage("QuestionId is required.");

			RuleFor(x => x.SelectedOptionId)
				.GreaterThan(0)
				.WithMessage("SelectedOptionId is required.");
		}
	}
}
