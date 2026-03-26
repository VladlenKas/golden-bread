using GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
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
