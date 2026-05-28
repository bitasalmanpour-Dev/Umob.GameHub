using FluentValidation;

namespace Umob.GameHub.Application.Authentication.RegisterUser
{
	public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
	{
		public RegisterUserCommandValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress()
				.MaximumLength(256);

			RuleFor(x => x.Password)
				.NotEmpty()
				.MinimumLength(8)
				.MaximumLength(100);

			RuleFor(x => x.Username)
				.MaximumLength(100);
		}
	}
}
