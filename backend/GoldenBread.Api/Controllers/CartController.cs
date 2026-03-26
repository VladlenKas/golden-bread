using GoldenBread.Application.Features.CompanyCart.Queries.GetCart;
using GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost("get-cart")]
    [Authorize]
    public async Task<IActionResult> GetCart([FromBody] GetCartQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    [HttpPost("create-order")]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand request)
    {
        var command = new CreateOrderCommand(
            request.DesiredDeliveryDate,
            request.TariffId);

        var result = await mediator.Send(command);

        if (result.InsufficientIngredients)
        {
            return Ok(new
            {
                success = false,
                insufficientIngredients = true,
            });
        }

        return Ok(new
        {
            success = true,
            orderId = result.OrderId
        });
    }
}
    