using MediatR;
using Umob.GameHub.Application.Authentication.Dtos;

namespace Umob.GameHub.Application.Authentication.LoginUser
{
	public sealed record LoginUserCommand(
	string Email,
	string Password
	) : IRequest<AuthResponse>;
}
