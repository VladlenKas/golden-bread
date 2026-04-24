using GoldenBread.Application.Contracts;
using GoldenBread.Application.Features.Auth.Commands.Login;
using GoldenBread.Application.Features.Auth.Commands.Logout;
using GoldenBread.Application.Features.Auth.Commands.RegisterCompany;
using GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login/{accountType}")]
    public async Task<IActionResult> Login(
        PortalType accountType, 
        [FromBody] LoginCommand command)
    {
        command = command with { AccountType = accountType };
        return Ok(await mediator.Send(command));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await mediator.Send(new LogoutCommand());
        return NoContent();
    }

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        await mediator.Send(command);
        return Created();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var query = new GetAccountBySessionQuery();
        var result = await mediator.Send(query);
        return result == null ? NoContent() : Ok(result);
    }
}
    