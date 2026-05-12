using GoldenBread.Application.Features.Ingredient.Commands.CreateIngredient;
using GoldenBread.Application.Features.Ingredient.Commands.DeleteIngredient;
using GoldenBread.Application.Features.Ingredient.Commands.UpdateIngredient;
using GoldenBread.Application.Features.Ingredient.Dtos;
using GoldenBread.Application.Features.Ingredient.Queries.GetIngredientById;
using GoldenBread.Application.Features.Ingredient.Queries.GetIngredientsList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/ingredients")]
[ApiController]
public class IngredientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetIngredientsListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetIngredientByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] IngredientDto dto)
    {
        var id = await mediator.Send(new CreateIngredientCommand(dto));
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] IngredientDto dto)
    {
        var result = await mediator.Send(new UpdateIngredientCommand(dto));
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteIngredientCommand(id));
        return result ? NoContent() : NotFound();
    }
}