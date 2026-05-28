using FluentValidation;

namespace Umob.GameHub.Application.Game.StartGame
{
	public sealed class StartGameCommandValidator
		: AbstractValidator<StartGameCommand>
	{
		public StartGameCommandValidator()
		{
			RuleFor(x => x.UserId)
				.GreaterThan(0)
				.WithMessage("UserId is required.");
		}
	}
}
