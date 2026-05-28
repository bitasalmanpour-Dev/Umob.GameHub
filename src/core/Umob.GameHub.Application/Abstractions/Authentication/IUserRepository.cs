using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.Authentication
{
	public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task InsertAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
