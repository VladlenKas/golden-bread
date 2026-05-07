using GoldenBread.Application.Features.Companies.Commands.CreateCompany;
using GoldenBread.Application.Features.Companies.Commands.UpdateCompany;
using GoldenBread.Application.Features.Companies.Dtos;
using GoldenBread.Application.Features.Companies.Queries.GetCompaniesList;
using GoldenBread.Application.Features.Companies.Queries.GetCompanyById;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetCompaniesListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetCompanyByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] CreateCompanyCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] CompanyDto dto)
    {
        var result = await mediator.Send(new UpdateCompanyCommand(dto));
        return result ? NoContent() : NotFound();
    }
}
