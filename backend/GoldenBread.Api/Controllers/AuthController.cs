using GoldenBread.Application.Abstractions.Enums;
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
    [HttpPost("login/company")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginCommand command)
    {
        command = command with { PortalType = PortalType.Company };
        return Ok(await mediator.Send(command));
    }

    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginCommand command)
    {
        command = command with { PortalType = PortalType.User };
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
        return Ok(await mediator.Send(new GetAccountBySessionQuery()));
    }
}
