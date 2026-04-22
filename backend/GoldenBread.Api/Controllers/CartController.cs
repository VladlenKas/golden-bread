using GoldenBread.Application.Features.CompanyCart.Commands.ToggleSelected;
using GoldenBread.Application.Features.CompanyCart.Commands.UpdateCartItem;
using GoldenBread.Application.Features.CompanyCart.Queries.GetCart;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCart()
    {
        return Ok(await mediator.Send(new GetCartQuery()));
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

    [HttpPatch("{id}/selection")]
    [Authorize]
    public async Task<IActionResult> ToggleSelected(
        int id,
        [FromBody] ToggleSelectedCommand command)
    {
        command = command with { ProductId = id };
        return Ok(await mediator.Send(command));
    }
}
    