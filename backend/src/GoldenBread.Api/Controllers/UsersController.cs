using GoldenBread.Application.Features.Users.Commands.CreateUser;
using GoldenBread.Application.Features.Users.Commands.UpdateUser;
using GoldenBread.Application.Features.Users.Dtos;
using GoldenBread.Application.Features.Users.Queries.GetUserById;
using GoldenBread.Application.Features.Users.Queries.GetUsersList;
using Microsoft.AspNetCore.Authorization;

namespace GoldenBread.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
        => Ok(await mediator.Send(new GetUsersListQuery()));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] UserDto dto)
    {
        var result = await mediator.Send(new UpdateUserCommand(dto));
        return result ? NoContent() : NotFound();
    }
}