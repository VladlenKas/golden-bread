using GoldenBread.Application.Features.Favorites.Commands.ToggleFavorite;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/favorites")]
[ApiController]
[Authorize]
public class FavoritesController(IMediator mediator) : ControllerBase
{
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var command = new ToggleFavoriteCommand(id);
        await mediator.Send(command);
        return NoContent();
    }
}
