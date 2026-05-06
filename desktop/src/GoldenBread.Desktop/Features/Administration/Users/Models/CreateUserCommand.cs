namespace GoldenBread.Desktop.Features.Administration.Users.Models;

public record CreateUserCommand(UserDto UserDto, string Email, string Password);
