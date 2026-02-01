using GoldenBread.Contracts.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Features.Auth.Commands.LoginUser;

public record class LoginUserCommand : IRequest<LoginUserResponse?>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
