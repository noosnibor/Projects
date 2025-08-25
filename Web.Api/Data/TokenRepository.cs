using DataAccess.Library;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Web.Api.Domain;

namespace Web.Api.Data;

public class TokenRepository(IConfiguration configuration)
{
    #region Data Access
    private readonly Sql _db = new() { Database = "Default", Configuration = configuration, Server = ServerOption.SqlServer };
    #endregion;

    #region Hash
    private static byte[] Hash(string token)
    {
        // Return hash data using the SHA256 algorithm.
        return SHA256.HashData(Encoding.UTF8.GetBytes(token));
    }
    #endregion

    #region Create refresh token
    public async Task CreateRefreshToken(Guid userId, string token, DateTime issued, DateTime expires, string device, string ip)
    {
        // Generate script to select a user Id
        string script = "INSERT INTO tblRefreshTokens (UserId, TokenHash, IssuedAt, ExpiresAt, Device, IpAddress)" +
                        "VALUES (@UserId, @TokenHash, @IssuedAt, @ExpiresAt, @Device, @IpAddress)";

        // Insert data in user role db
        await _db.AddAsync(script, CommandType.Text, new { UserId = userId, TokenHash = token, IssueAt = issued, ExpiresAt = expires, Device = device, IpAddress = ip });
    }
    #endregion

    #region Find active refresh token
    public async Task<RefreshTokenModel> FindActiveRefreshToken(string token)
    {
        // Generate script to select a user Id
        string script = "SELECT TOP 1 * " +
                        "FROM [tblRefreshTokens] r" +
                        "WHERE r.TokenHash = @Hash AND r.RevokedAt IS NULL AND r.ExpiresAt > SYSUTCDATETIME()";

        // Retrieve the result from the db
        List<RefreshTokenModel> result = await _db.GetAsync<RefreshTokenModel, dynamic>(script, CommandType.Text, new { Hash = Hash(token) });

        // Return the results
        return result.FirstOrDefault()!;
    }
    #endregion

    #region Rotate refresh token
    public async Task RotateRefreshToken(Guid currentToken, string newToken)
    {
        // Generate script to select a user Id
        string script = "Update [tblRefreshTokens]" +
                        "SET RevokedAt = SYSUTCDATETIME(), ReplacedByTokenHash = @NewHash" +
                        "WHERE Id = @Id";

        // Update the result from the db
        await _db.UpdateAsync(script, CommandType.Text, new { Id = currentToken, NewHashId = Hash(newToken) });
    }
    #endregion

    #region Revoked all token for user
    public async Task RevokedAllForUser(Guid userId, string? device = null)
    {
        // Generate script to select a user Id
        string script = "Update [tblRefreshTokens]" +
                        "SET RevokedAt = SYSUTCDATETIME()" +
                        "WHERE UserId = @UserId AND RevokedAt IS NULL AND (@Device IS NULL OR Device = @Device)";

        // Update the result from the db
        await _db.UpdateAsync(script, CommandType.Text, new { UserId = userId, Device = device });
    }
    #endregion

    #region Add One-time token for email confirm and password reset
    public async Task AddOneTimeToken(Guid userId, string purpose, string token, DateTime expires)
    {
        // Generate script to select a user Id
        string script = "INSERT INTO [tblOneTimeToken] (UserId, Purpose, TokenHash, ExpiresAt) " +
                        "VALUES (@UserId, @Purpose, @TokenHash, @ExpiresAt)";

        // Add the result from the db
        await _db.AddAsync(script, CommandType.Text, new { UserId = userId, Purpose = purpose, TokenHash = Hash(token), ExpiresAt = expires });
    }
    #endregion

    #region Consume the one time token
    public async Task<(OneTimeTokenModel? token, Guid? userId)> ConsumeOneTimeToken(string purpose, string token)
    {
        // Generate script to select a user Id
        string script = "SELECT TOP 1 * " +
                        "FROM tblOneTimeToken o " +
                        "WHERE o.Purpose = @Purpose AND o.TokenHash = @TokenHash AND o.ConsumedAt IS NULL AND o.ExpiresAt > SYSUTCDATETIME() ";

        // Retrieve the result from the db
        var result = await _db.GetAsync<OneTimeTokenModel, dynamic>(script, CommandType.Text, new { Purpose = purpose, TokenHash = Hash(token) });

        // Check if the result is null
        if (result is null)
        {
            return (null, null);
        }
        else
        {
            // Generate script to select a user Id
            string updateScript = "Update [tblOneTimeToken] " +
                                  "SET ConsumedAt = SYSUTCDATETIME() " +
                                  "WHERE Id = @Id";

            // Update the result from the db
            await _db.UpdateAsync(updateScript, CommandType.Text, new { result.FirstOrDefault()?.Id });

            // Return the token  
            return (result.FirstOrDefault(), result.FirstOrDefault()?.UserId);
        }

        

        
    }
    #endregion
}
