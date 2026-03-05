using GoldenBread.Application.Features.ProductCatalog.Queires.GetCatalog;
using GoldenBread.Application.Features.ProductCatalog.Queires.GetProductDetail;
using Microsoft.AspNetCore.Authorization;
using GoldenBread.Application.Features.CompanyCart.Commands.UpdateCartItem;
using GoldenBread.Application.Features.ProductFavorites.Commands.ToggleFavorite;

namespace GoldenBread.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var command = new GetCatalogQuery();
        return Ok(await mediator.Send(command));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await mediator.Send(new GetProductDetailQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var command = new ToggleFavoriteCommand(id);
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/update-cart")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItem(
        int id, 
        [FromBody] UpdateCartItemCommand command)
    {
        command = command with { ProductId = id };
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
