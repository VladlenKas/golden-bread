using GoldenBread.Application.Features.SupplierIngredient.Commands.CreateSupplierIngredient;
using GoldenBread.Application.Features.SupplierIngredient.Commands.DeleteSupplierIngredient;
using GoldenBread.Application.Features.SupplierIngredient.Commands.UpdateSupplierIngredient;
using GoldenBread.Application.Features.SupplierIngredient.Dtos;
using GoldenBread.Application.Features.SupplierIngredient.Queries.GetSupplierIngredientById;
using GoldenBread.Application.Features.SupplierIngredient.Queries.GetSupplierIngredientsList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/supplier-ingredients")]
[ApiController]
public class SupplierIngredientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetSupplierIngredientsListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetSupplierIngredientByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] SupplierIngredientDto dto)
    {
        var id = await mediator.Send(new CreateSupplierIngredientCommand(dto));
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, [FromBody] SupplierIngredientDto dto)
    {
        var result = await mediator.Send(new UpdateSupplierIngredientCommand(dto));
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteSupplierIngredientCommand(id));
        return result ? NoContent() : NotFound();
    }
}