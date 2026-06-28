using GoldenBread.Application.Features.EmployeeTasks.Commands;
using GoldenBread.Application.Features.EmployeeTasks.Dtos;
using GoldenBread.Application.Features.EmployeeTasks.Queries;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/employee-tasks")]
[ApiController]
[Authorize]
public class EmployeeTasksController(IMediator mediator) : ControllerBase
{
    [HttpGet("kanban")]
    public async Task<ActionResult<List<EmployeeTaskKanbanItem>>> GetKanban()
    {
        var result = await mediator.Send(new GetEmployeeTasksKanbanQuery());
        return Ok(result);
    }

    [HttpPut("status")]
    public async Task<ActionResult> UpdateStatus(
        [FromBody] UpdateEmployeeTaskStatusRequest request)
    {
        try
        {
            await mediator.Send(new UpdateEmployeeTaskStatusCommand(request));
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("progress")]
    public async Task<ActionResult> UpdateProgress(
    [FromBody] UpdateEmployeeTaskProgressRequest request)
    {
        try
        {
            await mediator.Send(new UpdateEmployeeTaskProgressCommand(request));
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeTaskDetailResponse>> GetById(int id)
    {
        var result = await mediator.Send(new GetEmployeeTaskByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}