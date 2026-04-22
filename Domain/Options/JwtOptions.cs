namespace Domain.Options
{
    public class JwtOptions
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
        public int AccessTokenExpirationMinutes { get; set; } = 5;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
