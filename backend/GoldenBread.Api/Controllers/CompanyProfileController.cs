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

    // GET api/company-profile/profile
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        return Ok(await mediator.Send(new GetProfileQuery()));
    }

    // PUT api/company-profile/requisites — блокирует аккаунт
    [HttpPut("requisites")]
    [Authorize]
    public async Task<IActionResult> UpdateRequisites([FromBody] UpdateRequisitesCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/company-profile/contacts
    [HttpPut("contacts")]
    [Authorize]
    public async Task<IActionResult> UpdateContacts([FromBody] UpdateContactsCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/company-profile/password
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/company-profile/email
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
