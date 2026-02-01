using GoldenBread.Application.Interfaces;
using GoldenBread.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoldenBread.Contracts.Requests;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController(IAccountUseCase accountUseCase) : ControllerBase
{
    [HttpPost("user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
    {
        var user = await accountUseCase.LoginUserAsync(request);
        return user == null ? Unauthorized() : Ok(user);
    }

    [HttpPost("company")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginRequest request)
    {
        var user = await accountUseCase.LoginCompanyAsync(request);
        return user == null ? Unauthorized() : Ok(user);
    }
}
