using GoldenBread.Application.Features.Employees.Commands.CreateEmployee;
using GoldenBread.Application.Features.Employees.Commands.DeleteEmployee;
using GoldenBread.Application.Features.Employees.Commands.UpdateEmployee;
using GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;
using GoldenBread.Application.Features.Employees.Queries.GetEmployeesList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/employees")]
[ApiController]
public class EmployeesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetEmployeesListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetEmployeeByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] CreateEmployeeCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateEmployeeCommand command)
    {
        var result = await mediator.Send(command with { EmployeeId = id });
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteEmployeeCommand(id));
        return result ? NoContent() : NotFound();
    }
}