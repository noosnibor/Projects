namespace Web.Api.Domain;

public record UserModel (Guid Id, string Email, bool EmailConfirmed, string PasswordHash, DateTime CreateAt, DateTime UpdatedAt)
{
    public UserModel() : this(default, default!, default, default!, default, default)
    {

    }
};
public record RoleModel (Guid Id, string Name)
{
    public RoleModel() : this(default, default!)
    {

    }
};
public record RefreshTokenModel (Guid Id, Guid UserId, byte[] TokenHash, DateTime IssuedAt, DateTime ExpiresAt, string Device, string IpAddress, DateTime RevokedAt, byte[] ReplacedByTokenHash)
{
    public RefreshTokenModel() : this(default, default!, default!, default!, default, default!, default!, default, default!)
    {

    }
};
public record OneTimeTokenModel (Guid Id, Guid UserId, string Purpose, byte[] TokenHash, DateTime ExpiresAt, DateTime ConsumedAt)
{
    public OneTimeTokenModel() : this(default, default!, default!, default!, default, default)
    {

    }
};
