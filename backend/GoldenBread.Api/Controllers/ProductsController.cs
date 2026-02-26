using GoldenBread.Application.Features.ProductCatalog.Commands.UpdateCartItem;
using GoldenBread.Application.Features.ProductCatalog.Commands.ToggleFavorite;
using GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    // GET api/products
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var command = new GetProductsListQuery();
        return Ok(await mediator.Send(command));
    }

    // PATCH api/products/{productId}/favourite
    [HttpPatch("{productId}/favorite")]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int productId)
    {
        var command = new ToggleFavoriteCommand(productId);
        await mediator.Send(command);
        return NoContent();
    }

    // PATCH api/products/{productId}/cart
    [HttpPatch("{productId}/cart")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItem(
        int productId, 
        [FromBody] UpdateCartItemCommand command)
    {
        command = command with { ProductId = productId };
        var result = await mediator.Send(command);
        return Ok(result);
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<ClientProductDetailResponse>> GetDetail(int id)
    //{
    //    var result = await mediator.Send(new GetProductDetailQuery(id));
    //    return result == null ? NotFound() : Ok(result);
    //}
}
