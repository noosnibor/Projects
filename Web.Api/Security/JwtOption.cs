namespace Web.Api.Security;

public class JwtOption
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int AccessTokenMinutes { get; set; } = 10;
    public int RefreshTokenDays { get; set; } = 14;
    public string SigningKey { get; set; } = default!;
}
