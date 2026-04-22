namespace Domain.Entities.Identity;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Token { get; set; }
    public required string JwtId { get; set; }
    public required bool IsRevoked { get; set; }
    public required DateTime DateAdded { get; set; }
    public required DateTime DateExpire { get; set; }
    public Guid UserId { get; set; }
    public required ApplicationUser User { get; set; }
}
