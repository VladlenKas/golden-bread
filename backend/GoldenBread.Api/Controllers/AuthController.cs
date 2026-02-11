using GoldenBread.Application.Features.Auth.Commands.Login;
using GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginCommand command)
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
