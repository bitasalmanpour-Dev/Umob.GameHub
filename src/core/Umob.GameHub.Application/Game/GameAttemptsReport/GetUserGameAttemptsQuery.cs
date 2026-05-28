using MediatR;
using Umob.GameHub.Application.Game.GameAttemptsReport.Dtos;

namespace Umob.GameHub.Application.Game.GameAttemptsReport
{
	public sealed record GetUserGameAttemptsQuery(long UserId)
		: IRequest<IReadOnlyList<GetUserGameAttemptsResponse>>;
}
