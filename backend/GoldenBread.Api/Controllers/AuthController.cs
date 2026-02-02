using GoldenBread.Application.Features.Auth.Commands.LoginCompany;
using GoldenBread.Application.Features.Auth.Commands.LoginUser;
using GoldenBread.Application.Features.Auth.Commands.RegisterCompany;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command)
    {
        var result = await mediator.Send(command);
        return result == null ? Unauthorized() : Ok(result);
    }

    [HttpPost("login/company")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginCompanyCommand command)
    {
        var result = await mediator.Send(command);
        return result == null ? Unauthorized() : Ok(result);
    }

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyCommand command)
    {
        await mediator.Send(command);
        return Created();
    }
}
