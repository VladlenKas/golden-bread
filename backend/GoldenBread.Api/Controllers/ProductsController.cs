using GoldenBread.Application.Features.ProductCatalog.Commands.ToggleFavourite;
using GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;

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

    // GET api/products/1/favourite
    [HttpPost("{productId}/favourite")]
    public async Task<IActionResult> ToggleFavourite(int productId)
    {
        var command = new ToggleFavouriteCommand(productId);
        await mediator.Send(command);
        return Ok();
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<ClientProductDetailResponse>> GetDetail(int id)
    //{
    //    var result = await mediator.Send(new GetProductDetailQuery(id));
    //    return result == null ? NotFound() : Ok(result);
    //}
}
