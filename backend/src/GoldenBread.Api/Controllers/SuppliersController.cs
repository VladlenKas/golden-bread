using GoldenBread.Application.Features.Suppliers.Commands.CreateSupplier;
using GoldenBread.Application.Features.Suppliers.Commands.DeleteSupplier;
using GoldenBread.Application.Features.Suppliers.Commands.UpdateSupplier;
using GoldenBread.Application.Features.Suppliers.Dtos;
using GoldenBread.Application.Features.Suppliers.Queries.GetSupplierById;
using GoldenBread.Application.Features.Suppliers.Queries.GetSuppliersList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/suppliers")]
[ApiController]
public class SuppliersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetSuppliersListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetSupplierByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] SupplierDto dto)
    {
        var id = await mediator.Send(new CreateSupplierCommand(dto));
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] SupplierDto dto)
    {
        var result = await mediator.Send(new UpdateSupplierCommand(dto));
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteSupplierCommand(id));
        return result ? NoContent() : NotFound();
    }
}