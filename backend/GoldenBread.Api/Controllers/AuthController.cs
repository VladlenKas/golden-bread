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
    // POST api/login 
    [HttpPost("login/{portalType}")]
    public async Task<IActionResult> LoginUser(
        PortalType portalType, 
        [FromBody] LoginCommand command)
    {
        command = command with { PortalType = portalType };
        return Ok(await mediator.Send(command));
    }

    // POST api/logout 
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await mediator.Send(new LogoutCommand());
        return NoContent();
    }

    // POST api/register/company 
    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        await mediator.Send(command);
        return Created();
    }

    // GET api/me 
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var query = new GetAccountBySessionQuery();
        var result = await mediator.Send(query);
        return result == null ? NoContent() : Ok(result);
    }
}
