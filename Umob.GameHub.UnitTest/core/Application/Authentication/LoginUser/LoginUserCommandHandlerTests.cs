using Moq;
using NUnit.Framework;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Application.Authentication.LoginUser;
using Umob.GameHub.Application.UnitTests.TestHelpers;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.UnitTests.Authentication.LoginUser;

[TestFixture]
public sealed class LoginUserCommandHandlerTests
{
	[Test]
	public async Task Handle_WhenEmailAndPasswordAreValid_ShouldReturnAuthResponse()
	{
		// Arrange
		var email = "bita@example.com";
		var password = "123456789!";
		var normalizedEmail = User.NormalizeEmail(email);

		var user = CreateUser(
			id: 1,
			email: normalizedEmail,
			username: "bita");

		var userRepository = new Mock<IUserRepository>();
		var passwordHasher = new Mock<IPasswordHasher>();
		var jwtToken = new Mock<IJwtToken>();

		userRepository
			.Setup(x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		passwordHasher
			.Setup(x => x.VerifyPassword(user, password))
			.Returns(true);

		jwtToken
			.Setup(x => x.GenerateToken(user))
			.Returns("fake-access-token");

		var handler = new LoginUserCommandHandler(
			userRepository.Object,
			passwordHasher.Object,
			jwtToken.Object);

		var command = new LoginUserCommand(
			email,
			password);

		// Act
		var response = await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		Assert.That(response, Is.Not.Null);
		Assert.That(response.UserId, Is.EqualTo(user.Id));
		Assert.That(response.Email, Is.EqualTo(user.Email));
		Assert.That(response.Username, Is.EqualTo(user.Username));
		Assert.That(response.AccessToken, Is.EqualTo("fake-access-token"));

		userRepository.Verify(
			x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()),
			Times.Once);

		passwordHasher.Verify(
			x => x.VerifyPassword(user, password),
			Times.Once);

		jwtToken.Verify(
			x => x.GenerateToken(user),
			Times.Once);
	}

	[Test]
	public void Handle_WhenUserDoesNotExist_ShouldThrowUnauthorizedAccessException()
	{
		// Arrange
		var email = "unknown@example.com";
		var password = "Password123!";
		var normalizedEmail = User.NormalizeEmail(email);

		var userRepository = new Mock<IUserRepository>();
		var passwordHasher = new Mock<IPasswordHasher>();
		var jwtToken = new Mock<IJwtToken>();

		userRepository
			.Setup(x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		var handler = new LoginUserCommandHandler(
			userRepository.Object,
			passwordHasher.Object,
			jwtToken.Object);

		var command = new LoginUserCommand(
			email,
			password);

		// Act & Assert
		var exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
			async () => await handler.Handle(
				command,
				CancellationToken.None));

		Assert.That(exception!.Message, Is.EqualTo("Invalid email or password."));

		passwordHasher.Verify(
			x => x.VerifyPassword(
				It.IsAny<User>(),
				It.IsAny<string>()),
			Times.Never);

		jwtToken.Verify(
			x => x.GenerateToken(It.IsAny<User>()),
			Times.Never);
	}

	[Test]
	public void Handle_WhenPasswordIsInvalid_ShouldThrowUnauthorizedAccessException()
	{
		// Arrange
		var email = "bita@example.com";
		var password = "WrongPassword";
		var normalizedEmail = User.NormalizeEmail(email);

		var user = CreateUser(
			id: 1,
			email: normalizedEmail,
			username: "bita");

		var userRepository = new Mock<IUserRepository>();
		var passwordHasher = new Mock<IPasswordHasher>();
		var jwtToken = new Mock<IJwtToken>();

		userRepository
			.Setup(x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		passwordHasher
			.Setup(x => x.VerifyPassword(user, password))
			.Returns(false);

		var handler = new LoginUserCommandHandler(
			userRepository.Object,
			passwordHasher.Object,
			jwtToken.Object);

		var command = new LoginUserCommand(
			email,
			password);

		// Act & Assert
		var exception = Assert.ThrowsAsync<UnauthorizedAccessException>(
			async () => await handler.Handle(
				command,
				CancellationToken.None));

		Assert.That(exception!.Message, Is.EqualTo("Invalid email or password."));

		jwtToken.Verify(
			x => x.GenerateToken(It.IsAny<User>()),
			Times.Never);
	}

	[Test]
	public async Task Handle_ShouldUseNormalizedEmail_WhenSearchingUser()
	{
		// Arrange
		var email = "  BITA@EXAMPLE.COM  ";
		var password = "123456789";
		var normalizedEmail = User.NormalizeEmail(email);

		var user = CreateUser(
			id: 1,
			email: normalizedEmail,
			username: "bita");

		var userRepository = new Mock<IUserRepository>();
		var passwordHasher = new Mock<IPasswordHasher>();
		var jwtToken = new Mock<IJwtToken>();

		userRepository
			.Setup(x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		passwordHasher
			.Setup(x => x.VerifyPassword(user, password))
			.Returns(true);

		jwtToken
			.Setup(x => x.GenerateToken(user))
			.Returns("fake-access-token");

		var handler = new LoginUserCommandHandler(
			userRepository.Object,
			passwordHasher.Object,
			jwtToken.Object);

		var command = new LoginUserCommand(
			email,
			password);

		// Act
		await handler.Handle(
			command,
			CancellationToken.None);

		// Assert
		userRepository.Verify(
			x => x.GetByEmailAsync(
				normalizedEmail,
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	private static User CreateUser(
		long id,
		string email,
		string username)
	{
		var user = ReflectionHelper.CreateInstance<User>();

		ReflectionHelper.SetPrivateProperty(user, nameof(User.Id), id);
		ReflectionHelper.SetPrivateProperty(user, nameof(User.Email), email);
		ReflectionHelper.SetPrivateProperty(user, nameof(User.Username), username);
		ReflectionHelper.SetPrivateProperty(user, nameof(User.PasswordHash), "hashed-password");
		ReflectionHelper.SetPrivateProperty(user, nameof(User.CreatedOn), DateTime.UtcNow);

		return user;
	}
}