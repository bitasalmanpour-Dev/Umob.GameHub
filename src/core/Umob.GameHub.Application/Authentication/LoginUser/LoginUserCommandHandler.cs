using MediatR;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Application.Authentication.Dtos;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Authentication.LoginUser
{
	public sealed class LoginUserCommandHandler(IUserRepository userRepository,
			IPasswordHasher passwordHashingService,
			IJwtToken jwtTokenService)
			: IRequestHandler<LoginUserCommand, AuthResponse>
	{
		public async Task<AuthResponse> Handle(
			LoginUserCommand request,
			CancellationToken cancellationToken)
		{
			var normalizedEmail = User.NormalizeEmail(request.Email);

			var user = await userRepository.GetByEmailAsync(
				normalizedEmail,
				cancellationToken);

			if (user is null)
			{
				throw new UnauthorizedAccessException("Invalid email or password.");
			}

			var passwordIsValid = passwordHashingService.VerifyPassword(
				user,
				request.Password);

			if (!passwordIsValid)
			{
				throw new UnauthorizedAccessException("Invalid email or password.");
			}

			var accessToken = jwtTokenService.GenerateToken(user);

			return new AuthResponse(
				user.Id,
				user.Email,
				user.Username,
				accessToken);
		}
	}
}
