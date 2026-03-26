using GoldenBread.Application.Features.Catalog.Queires.GetCatalog;
using GoldenBread.Application.Features.Catalog.Queires.GetProductDetail;

namespace GoldenBread.Api.Controllers;

[Route("api/catalog")]
[ApiController]
public class CatalogController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await mediator.Send(new GetCatalogQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await mediator.Send(new GetProductDetailQuery(id));
        return result == null ? NotFound() : Ok(result);
    }
}