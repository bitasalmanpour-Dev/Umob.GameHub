using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.Authentication
{
	public interface IPasswordHasher
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(User user, string password);
    }
}
