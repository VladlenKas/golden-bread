using GoldenBread.Application.Features.Document.Commands;
using GoldenBread.Application.Features.Document.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/document")]
[ApiController]
[Authorize]
public class DocumentController(IMediator mediator) : ControllerBase
{
    [HttpPost("delivery-invoice-pdf/{id}")]
    [Authorize]
    public async Task<IActionResult> CreateDeliveryInvoiceXlsx(int id, int userId)
    {
        var command = new GenerateDeliveryInvoiceCommand(id, userId);
        var result = await mediator.Send(command);
        return File(result.FileBytes, result.ContentType, result.FileName);
    }

    [HttpPost("cooperation-agreement-pdf/{id}")]
    [Authorize]
    public async Task<IActionResult> GetCooperationAgreement(int id)
    {
        var command = new GenerateCooperationAgreementCommand(id);
        var result = await mediator.Send(command);
        return File(result.FileBytes, "application/pdf", result.FileName);
    }

    [HttpPost("cooperation-agreement-pdf/public")]
    [AllowAnonymous] 
    public IActionResult GetCooperationAgreementForRegistration([FromBody] CompanyInfoDto dto)
    {
        var command = new GenerateCooperationAgreementForRegisterCommand(dto);
        var result = mediator.Send(command).Result; 
        return File(result.FileBytes, "application/pdf", result.FileName);
    }
}

