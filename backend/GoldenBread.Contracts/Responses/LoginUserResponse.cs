namespace GoldenBread.Contracts.Responses;

public class LoginUserResponse
{
    public required int Id { get; set; }
    public required string Fullname { get; set; } 
    public required string Role { get; set; } 
    public required string AccountStatus { get; set; }
    public required string Session { get; set; }
    public required DateTime SessionExpiresAt { get; set; }
}
