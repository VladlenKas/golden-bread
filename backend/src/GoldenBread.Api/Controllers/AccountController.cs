using GoldenBread.Application.Features.Account.Commands.DeleteAccount;
using GoldenBread.Application.Features.Account.Commands.UpdateAccountEmail;
using GoldenBread.Application.Features.Account.Commands.UpdateAccountPassword;
using GoldenBread.Application.Features.Account.Commands.UpdateAccountStatus;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> PutPassword([FromBody] UpdateAccountPasswordCommand command)
    {
        var result = await mediator.Send(new UpdateAccountPasswordCommand(command.AccountId, command.NewPassword));
        return result ? NoContent() : NotFound();
    }

    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> PutEmail([FromBody] UpdateAccountEmailCommand command)
    {
        var result = await mediator.Send(new UpdateAccountEmailCommand(command.AccountId, command.NewEmail));
        return result ? NoContent() : NotFound();
    }

    [HttpPut("status")]
    [Authorize]
    public async Task<IActionResult> PutStatus([FromBody] UpdateAccountStatusCommand command)
    {
        var result = await mediator.Send(new UpdateAccountStatusCommand(command.AccountId, command.Status));
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteAccountCommand(id));
        return result ? NoContent() : NotFound();
    }
}
