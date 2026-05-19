using GoldenBread.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/images")]
[ApiController]
public class ImagesController(IFileStorage fileStorage) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        await using var stream = file.OpenReadStream();
        var fileName = await fileStorage.SaveAsync(stream, file.FileName);

        return Ok(new { FileName = fileName });
    }
}