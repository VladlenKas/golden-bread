using GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyEmail;
using GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyPassword;
using GoldenBread.Application.Features.CompanyAccount.Commands.UpdateMyContacts;
using GoldenBread.Application.Features.CompanyAccount.Commands.UpdateMyRequisites;
using GoldenBread.Application.Features.CompanyAccount.Queries.GetMyProfile;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/account-company")]
[ApiController]
public class MyCompanyController(IMediator mediator) : ControllerBase
{

    // GET api/account-company/profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        return Ok(await mediator.Send(new GetMyProfileQuery()));
    }

    // PUT api/account-company/requisites — блокирует аккаунт
    [HttpPut("requisites")]
    [Authorize]
    public async Task<IActionResult> UpdateRequisites([FromBody] UpdateMyRequisitesCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/account-company/contacts
    [HttpPut("contacts")]
    [Authorize]
    public async Task<IActionResult> UpdateContacts([FromBody] UpdateMyContactsCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/account-company/password
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeMyPasswordCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    // PUT api/account-company/email — блокирует аккаунт
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeMyEmailCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
