using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Umob.GameHub.Application.Abstractions.Authentication;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Infrastructure.Authentication
{
	public sealed class JwtToken : IJwtToken
	{
		private readonly JwtOptions _jwtOptions;

		public JwtToken(IOptions<JwtOptions> jwtOptions)
		{
			_jwtOptions = jwtOptions.Value;
		}

		public string GenerateToken(User user)
		{
			var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Email, user.Email)
		};

			if (!string.IsNullOrWhiteSpace(user.Username))
			{
				claims.Add(new Claim("username", user.Username));
			}

			var securityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

			var credentials = new SigningCredentials(
				securityKey,
				SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtOptions.Issuer,
				audience: _jwtOptions.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
