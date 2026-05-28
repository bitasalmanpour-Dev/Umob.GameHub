using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Umob.GameHub.Application.Authentication.LoginUser;
using Umob.GameHub.Application.Authentication.RegisterUser;
using Umob.GameHub.Api.V1.Controllers.Dtos;

namespace Umob.GameHub.Api.V1.Controllers
{
	[ApiController]
	[Route("api/v1/auth")]
	public sealed class AuthenticationController : ControllerBase
	{
		private readonly ISender _sender;

		public AuthenticationController(ISender sender)
		{
			_sender = sender;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(
			RegisterUserCommand command,
			CancellationToken cancellationToken)
		{
			try
			{

				var response = await _sender.Send(command, cancellationToken);

				return Ok(response);
			}
			catch (ValidationException exception)
			{
				return BadRequest(new
				{
					message = "Validation failed.",
					errors = exception.Errors.Select(error => new
					{
						field = error.PropertyName,
						error = error.ErrorMessage
					})
				});
			}
			catch (InvalidOperationException exception)
			{
				return Conflict(new
				{
					message = exception.Message
				});
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(
			LoginUserRequest request,
			CancellationToken cancellationToken)
		{
			try
			{
				var response = await _sender.Send(
					new LoginUserCommand(request.Email,request.Password)
					, cancellationToken);

				return Ok(response);
			}
			catch (ValidationException exception)
			{
				return BadRequest(new
				{
					message = "Validation failed.",
					errors = exception.Errors.Select(error => new
					{
						field = error.PropertyName,
						error = error.ErrorMessage
					})
				});
			}
			catch (UnauthorizedAccessException exception)
			{
				return Unauthorized(new
				{
					message = exception.Message
				});
			}
		}
	}
}
