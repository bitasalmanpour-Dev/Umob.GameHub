using MediatR;

namespace Umob.GameHub.Application.Game.StartGame
{
	public sealed record StartGameCommand(long UserId)
		: IRequest<StartGameResponse?>;
}
