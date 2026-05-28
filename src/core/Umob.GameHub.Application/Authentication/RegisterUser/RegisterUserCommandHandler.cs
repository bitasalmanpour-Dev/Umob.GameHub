using MediatR;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Application.Authentication.Dtos;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Authentication.RegisterUser
{
	public sealed class RegisterUserCommandHandler(IUserRepository userRepository,
			IPasswordHasher passwordHashingService,
			IJwtToken jwtTokenService)
		    : IRequestHandler<RegisterUserCommand, AuthResponse>
	{
		public async Task<AuthResponse> Handle(
			RegisterUserCommand request,
			CancellationToken cancellationToken)
		{
			var normalizedEmail = User.NormalizeEmail(request.Email);

			var emailExists = await userRepository.ExistsByEmailAsync(
				normalizedEmail,
				cancellationToken);

			if (emailExists)
			{
				throw new InvalidOperationException("A user with this email already exists.");
			}

			var user = User.Create(normalizedEmail, request.Username);

			var passwordHash = passwordHashingService.HashPassword(
				user,
				request.Password);

			user.SetPasswordHash(passwordHash);

			await userRepository.InsertAsync(user, cancellationToken);

			var accessToken = jwtTokenService.GenerateToken(user);

			return new AuthResponse(
				user.Id,
				user.Email,
				user.Username,
				accessToken);
		}
	}
}
