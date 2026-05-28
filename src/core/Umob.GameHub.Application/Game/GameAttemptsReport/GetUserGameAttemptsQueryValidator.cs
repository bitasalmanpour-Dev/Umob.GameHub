using FluentValidation;

namespace Umob.GameHub.Application.Game.GameAttemptsReport
{
	public sealed class GetUserGameAttemptsQueryValidator
		: AbstractValidator<GetUserGameAttemptsQuery>
	{
		public GetUserGameAttemptsQueryValidator()
		{
			RuleFor(x => x.UserId)
				.GreaterThan(0)
				.WithMessage("UserId is required.");
		}
	}
}
