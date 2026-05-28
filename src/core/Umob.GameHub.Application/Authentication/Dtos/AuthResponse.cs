namespace Umob.GameHub.Application.Authentication.Dtos
{
	public sealed record AuthResponse(
        long UserId,
        string Email,
        string Username,
        string AccessToken
        );
}
