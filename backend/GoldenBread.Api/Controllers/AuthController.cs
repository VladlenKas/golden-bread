using GoldenBread.Application.Features.Auth.Commands.Login;
using GoldenBread.Application.Features.Auth.Commands.RegisterCompany;
using GoldenBread.Application.Features.Auth.Queries;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
        => Ok(await mediator.Send(command));

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
        => Ok(await mediator.Send(new LogoutCommand()));

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
        => CreatedAtAction(nameof(Me), await mediator.Send(command));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me() 
        => Ok(await mediator.Send(new GetAccountBySessionQuery()));
}
