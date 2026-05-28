namespace Umob.GameHub.Domain.Entities
{
	public class User
	{
		public long Id { get; private set; }
		public string Email { get; private set; }
		public string PasswordHash { get; private set; }
		public string Username { get; private set; }
		public DateTime CreatedOn { get; private set; }
		public DateTime? ModifiedOn { get; private set; }

		public User(string email, string username)
		{
			Email = NormalizeEmail(email);
			Username = username.Trim();
			PasswordHash = string.Empty;
			CreatedOn = DateTime.UtcNow;
		}

		public static User Create(string email, string username)
		{
			return new User(email, username);
		}
		public void SetPasswordHash(string passwordHash)
		{
			if (string.IsNullOrWhiteSpace(passwordHash))
			{
				throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
			}

			PasswordHash = passwordHash; 
			ModifiedOn = DateTime.UtcNow;
		}

		public static string NormalizeEmail(string email)
		{
			return email.Trim().ToLowerInvariant();
		}

        public User()
        {
            
        }
    }
}
