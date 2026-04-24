using GoldenBread.Application.Features.Favorites.Commands.ToggleFavorite;
using GoldenBread.Application.Features.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/favorites")]
[ApiController]
[Authorize]
public class FavoritesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetList()
    {
        return Ok(await mediator.Send(new GetFavoritesQuery()));
    }


    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var command = new ToggleFavoriteCommand(id);
        await mediator.Send(command);
        return NoContent();
    }
}
