using Umob.GameHub.Domain.Entities;
using Umob.GameHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Umob.GameHub.Application.Abstractions.Authentication;

namespace Umob.GameHub.Infrastructure.Repositories
{
	internal sealed class UserRepository(ApplicationDbContext dbContext)
		: IUserRepository
	{
		public Task<User?> GetByEmailAsync(
			string email,
			CancellationToken cancellationToken = default)
		{
			var normalizedEmail = User.NormalizeEmail(email);

			return dbContext.Users
				.FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
		}

		public Task<bool> ExistsByEmailAsync(
			string email,
			CancellationToken cancellationToken = default)
		{
			var normalizedEmail = User.NormalizeEmail(email);

			return dbContext.Users
				.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
		}

		public async Task InsertAsync(
			User user,
			CancellationToken cancellationToken = default)
		{
			 dbContext.Users.Add(user);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
