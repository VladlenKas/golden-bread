using GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;
using GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;
using GoldenBread.Application.Features.CompanyProfile.Commands.ChangePassword;
using GoldenBread.Application.Features.CompanyProfile.Commands.UpdateContacts;
using GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/company-profile")]
[ApiController]
public class CompanyProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await mediator.Send(new GetProfileQuery()));
    }

    [HttpPut("requisites")]
    [Authorize]
    public async Task<IActionResult> UpdateRequisites(
        [FromBody] UpdateRequisitesCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPut("contacts")]
    [Authorize]
    public async Task<IActionResult> UpdateContacts(
        [FromBody] UpdateContactsCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail(
        [FromBody] ChangeEmailCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
