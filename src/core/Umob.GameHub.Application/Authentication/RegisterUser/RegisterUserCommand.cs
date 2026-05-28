using MediatR;
using Umob.GameHub.Application.Authentication.Dtos;

namespace Umob.GameHub.Application.Authentication.RegisterUser
{
	public sealed record RegisterUserCommand(
	string Email,
	string Password,
	string Username
	) : IRequest<AuthResponse>;
}
