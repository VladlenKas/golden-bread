using GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController(IMediator mediator) : ControllerBase
{
    //[HttpGet]
    //[Authorize]
    //public async Task<IActionResult> GetCart()
    //{
    //    return Ok(await mediator.Send(new GetCartQuery()));
    //}
}
    