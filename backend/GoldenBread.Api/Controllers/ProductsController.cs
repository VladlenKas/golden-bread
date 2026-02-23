using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;

namespace GoldenBread.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await mediator.Send(new GetProductsListQuery()));
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<ClientProductDetailResponse>> GetDetail(int id)
    //{
    //    var result = await mediator.Send(new GetProductDetailQuery(id));
    //    return result == null ? NotFound() : Ok(result);
    //}
}
