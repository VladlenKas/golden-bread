using GoldenBread.Application.Interfaces;
using GoldenBread.Application.UseCases;
using GoldenBread.Contracts.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegisterController(IAccountUseCase accountUseCase) : ControllerBase
{
    [HttpPost("company")]
    public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyRequest request)
    {
        await accountUseCase.RegisterCompanyAsync(request);
        return Created();
    }
}
