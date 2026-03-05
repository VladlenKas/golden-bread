using GoldenBread.Application.Features.CompanyCart.Queries.GetCart;
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

    //[HttpPost("create-order")]
    //[Authorize]
    //public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    //{
    //    var command = new CreateOrderCommand(
    //        request.CartId,
    //        request.TariffId,
    //        request.DesiredDeliveryDate);

    //    var result = await mediator.Send(command);

    //    if (result.InsufficientIngredients)
    //    {
    //        return Ok(new
    //        {
    //            success = false,
    //            insufficientIngredients = true,
    //            missingIngredients = result.MissingIngredients
    //        });
    //    }

    //    return Ok(new
    //    {
    //        success = true,
    //        orderId = result.OrderId
    //    });
    //}
}
    