using MediatR;
using Microsoft.AspNetCore.Mvc;
using Umob.GameHub.Application.Game.StartGame;
using Umob.GameHub.Api.V1.Controllers.Dtos;
using Umob.GameHub.Application.Game.AnswerQuestion;
using Umob.GameHub.Application.Game.GameAttemptsReport;

namespace Umob.GameHub.Api.V1.Controllers
{
	[ApiController]
	[Route("api/v1/games")]
	public sealed class GamesController : ControllerBase
	{
		private readonly ISender _sender;

		public GamesController(ISender sender)
		{
			_sender = sender;
		}

		[HttpPost("start")]
		public async Task<IActionResult> StartGame(
			[FromBody] StartGameRequest request,
			CancellationToken cancellationToken)
		{
			if (request.UserId <= 0)
			{
				return BadRequest("UserId is required.");
			}

			var response = await _sender.Send(
				new StartGameCommand(request.UserId),
				cancellationToken);

			if (response is null)
			{
				return BadRequest(
					"Could not start game. Please check active questions, providers, and GBFS configuration.");
			}

			return Ok(response);
		}

		[HttpPost("answer")]
		public async Task<IActionResult> AnswerQuestion(
			[FromBody] AnswerQuestionRequest request,
			CancellationToken cancellationToken)
		{
			if (request.UserId <= 0)
			{
				return BadRequest("UserId is required.");
			}

			if (request.QuestionId <= 0)
			{
				return BadRequest("QuestionId is required.");
			}

			if (request.SelectedOptionId <= 0)
			{
				return BadRequest("SelectedOptionId is required.");
			}

			var response = await _sender.Send(
				new AnswerQuestionCommand(
					request.AttemptId,
					request.UserId,
					request.QuestionId,
					request.SelectedOptionId),
				cancellationToken);

			if (response is null)
			{
				return BadRequest(
					"Could not submit answer. The question may be already answered, or the selected option is invalid.");
			}

			return Ok(response);
		}

		[HttpGet("attempt")]
		public async Task<IActionResult> GetAttempts(
			[FromQuery] long userId,
			CancellationToken cancellationToken)
		{
			if (userId <= 0)
			{
				return BadRequest("UserId is required.");
			}

			var response = await _sender.Send(
				new GetUserGameAttemptsQuery(userId),
				cancellationToken);

			return Ok(response);
		}
	}
}
