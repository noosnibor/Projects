namespace Web.Api.Domain;

public record UserModel (Guid Id, string Email, bool EmailConfirmed, string PasswordHash, DateTime CreateAt, DateTime UpdatedAt);
public record RoleModel (Guid Id, string Name);
public record RefreshTokenModel (Guid Id, Guid UserId, byte[] TokenHash, DateTime IssuedAt, DateTime ExpiresAt, string Device, string IpAddress, DateTime RevokedAt, byte[] ReplacedByTokenHash);
public record OneTimeTokenModel (Guid Id, Guid UserId, string Purpose, byte[] TokenHash, DateTime ExpiresAt, DateTime ConsumedAt);
