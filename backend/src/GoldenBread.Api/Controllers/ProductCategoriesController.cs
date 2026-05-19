using GoldenBread.Application.Features.ProductCategory.Commands.CreateProductCategory;
using GoldenBread.Application.Features.ProductCategory.Commands.DeleteProductCategory;
using GoldenBread.Application.Features.ProductCategory.Commands.UpdateProductCategory;
using GoldenBread.Application.Features.ProductCategory.Dtos;
using GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoriesList;
using GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoryById;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/product-categories")]
[ApiController]
public class ProductCategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetProductCategoriesListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetProductCategoryByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] ProductCategoryDto dto)
    {
        var id = await mediator.Send(new CreateProductCategoryCommand(dto));
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] ProductCategoryDto dto)
    {
        var result = await mediator.Send(new UpdateProductCategoryCommand(dto));
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteProductCategoryCommand(id));
        return result ? NoContent() : NotFound();
    }
}