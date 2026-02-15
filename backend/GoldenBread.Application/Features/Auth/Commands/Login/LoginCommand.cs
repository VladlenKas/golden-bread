using GoldenBread.Application.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed record class LoginCommand(
    string Email,
    string Password,
    PortalType PortalType) : IRequest<AuthResponse>;