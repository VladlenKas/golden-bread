namespace GoldenBread.Desktop.Features.Administration.Users.Models;

public record CreateUserRequest(UserDto Dto, string Email, string Password);
