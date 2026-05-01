using DocumentFormat.OpenXml.Office2010.Excel;
using GoldenBread.Application.Features.Catalog.Queires.GetProductDetail;
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
    public void Post([FromBody] string value)
    {
    }

    [HttpPut("{id}")]
    [Authorize]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    [Authorize]
    public void Delete(int id)
    {
    }
}
