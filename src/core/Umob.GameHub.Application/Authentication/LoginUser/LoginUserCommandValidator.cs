using FluentValidation;

namespace Umob.GameHub.Application.Authentication.LoginUser
{
	public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
	{
		public LoginUserCommandValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress()
				.MaximumLength(256);

			RuleFor(x => x.Password)
				.NotEmpty()
				.MaximumLength(100);
		}
	}
}
