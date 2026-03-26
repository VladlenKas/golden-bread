using GoldenBread.Application.Features.CompanyCart.Commands.UpdateCartItem;
using GoldenBread.Application.Features.CompanyCart.Queries.GetCart;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetCart([FromBody] GetCartQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateItem(
        int id,
        [FromBody] UpdateCartItemCommand command)
    {
        command = command with { ProductId = id };
        return Ok(await mediator.Send(command));
    }
}
    