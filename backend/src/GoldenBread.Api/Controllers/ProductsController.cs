using GoldenBread.Application.Features.Catalog.Queires.GetCatalog;
using GoldenBread.Application.Features.Catalog.Queires.GetProductDetail;
using GoldenBread.Application.Features.Products.Commands.CreateProductWithDetails;
using GoldenBread.Application.Features.Products.Commands.DeleteProduct;
using GoldenBread.Application.Features.Products.Commands.UpdateProduct;
using GoldenBread.Application.Features.Products.Commands.UpdateProductBatches;
using GoldenBread.Application.Features.Products.Commands.UpdateProductImages;
using GoldenBread.Application.Features.Products.Commands.UpdateProductRecipe;
using GoldenBread.Application.Features.Products.Dtos;
using GoldenBread.Application.Features.Products.Queries.GetProductById;
using GoldenBread.Application.Features.Statistics.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetCatalogQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/detail")]
    [Authorize]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await mediator.Send(new GetProductDetailQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] CreateProductWithDetailsCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] ProductDto dto)
    {
        var result = await mediator.Send(new UpdateProductCommand(dto));
        return result ? NoContent() : NotFound();
    }

    [HttpPut("recipe")]
    [Authorize]
    public async Task<IActionResult> PutRecipe([FromBody] UpdateProductRecipeCommand command)
    {
        var result = await mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("batches")]
    [Authorize]
    public async Task<IActionResult> PutBatches([FromBody] UpdateProductBatchesCommand command)
    {
        var result = await mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("images")]
    [Authorize]
    public async Task<IActionResult> PutImages([FromBody] UpdateProductImagesCommand command)
    {
        var result = await mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id));
        return result ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpGet("raw-data")]
    public async Task<ActionResult<RawStatisticsData>> GetRawData(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRawStatisticsQuery(dateFrom, dateTo);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
