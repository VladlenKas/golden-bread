using GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;
using GoldenBread.Application.Features.CompanyOrder.Queries.GetOrders;
using GoldenBread.Application.Features.Orders.Commands;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Application.Features.Orders.Exceptions;
using GoldenBread.Application.Features.Orders.Queries;
using Microsoft.AspNetCore.Authorization;
using CreateOrderCommandUser = GoldenBread.Application.Features.Orders.Commands.CreateOrderCommand;
using CreateOrderCommandCompany = GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder.CreateOrderCommand;

namespace GoldenBread.Api.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetList()
    {
        return Ok(await mediator.Send(new GetOrdersQuery()));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrderCompany([FromBody] CreateOrderCommandCompany request)
    {
        var command = new CreateOrderCommandCompany(
            request.DesiredDeliveryDate);

        var result = await mediator.Send(command);

        return Ok(new
        {
            success = true,
            orderId = result.OrderId
        });
    }

    [HttpGet("kanban")]
    [Authorize]
    public async Task<ActionResult<List<OrderKanbanItem>>> GetKanban()
    {
        var result = await mediator.Send(new GetOrdersKanbanQuery());
        return Ok(result);
    }

    [HttpPut("status")]
    [Authorize]
    public async Task<ActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            await mediator.Send(new UpdateOrderStatusCommand(request));
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("editor-data")]
    [Authorize]
    public async Task<ActionResult<OrderEditorDataResponse>> GetEditorData()
    {
        var result = await mediator.Send(new GetOrderEditorDataQuery());
        return Ok(result);
    }

    [HttpPost("calculate-delivery")]
    [Authorize]
    public async Task<ActionResult<CalculateDeliveryResponse>> CalculateDelivery(
        [FromBody] CalculateDeliveryRequest request)
    {
        var result = await mediator.Send(new CalculateDeliveryDateQuery(request));
        return Ok(result);
    }

    [HttpPost("create-from-user")]
    [Authorize]
    public async Task<ActionResult<int>> CreateOrderUser([FromBody] CreateOrderRequest request)
    {
        var id = await mediator.Send(new CreateOrderCommandUser(request));
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDetailResponse>> GetById(int id)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
