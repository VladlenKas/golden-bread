using GoldenBread.Application.Features.Document.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GoldenBread.Api.Controllers;

[Route("api/document")]
[ApiController]
[Authorize]
public class DocumentController(IMediator mediator) : ControllerBase
{
    [HttpPost("delivery-invoice-xlsx/{id}")]
    [Authorize]
    public async Task<IActionResult> CreateDeliveryInvoiceXlsx(int id)
    {
        var command = new GenerateDeliveryInvoiceCommand(id);
        var result = await mediator.Send(command);
        return File(result.FileBytes, result.ContentType, result.FileName);
    }
}

