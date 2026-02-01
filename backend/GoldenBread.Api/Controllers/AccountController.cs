using GoldenBread.Application.Interfaces;
using GoldenBread.Application.UseCases;
using GoldenBread.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountUseCase accountUseCase) : ControllerBase
{
    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
    {
        var user = await accountUseCase.LoginUserAsync(request);
        return user == null ? Unauthorized() : Ok(user);
    }

    [HttpPost("login/company")]
    public async Task<IActionResult> LoginCompany([FromBody] LoginRequest request)
    {
        var user = await accountUseCase.LoginCompanyAsync(request);
        return user == null ? Unauthorized() : Ok(user);
    }

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyRequest request)
    {
        await accountUseCase.RegisterCompanyAsync(request);
        return Created();
    }
}
