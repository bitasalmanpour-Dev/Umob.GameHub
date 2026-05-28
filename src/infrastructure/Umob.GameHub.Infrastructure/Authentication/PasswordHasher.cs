using Microsoft.AspNetCore.Identity;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Authentication
{
	public sealed class PasswordHasher : IPasswordHasher
	{
		private readonly PasswordHasher<User> _passwordHasher = new();

		public string HashPassword(User user, string password)
		{
			return _passwordHasher.HashPassword(user, password);
		}

		public bool VerifyPassword(User user, string password)
		{
			var result = _passwordHasher.VerifyHashedPassword(
				user,
				user.PasswordHash,
				password);

			return result is PasswordVerificationResult.Success
				or PasswordVerificationResult.SuccessRehashNeeded;
		}
	}
}
