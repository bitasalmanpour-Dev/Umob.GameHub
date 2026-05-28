namespace Umob.GameHub.Api.V1.Controllers.Dtos
{
	public sealed record RegisterUserRequest(string Email,
		string Password,
		string Username);

}
