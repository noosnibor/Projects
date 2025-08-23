namespace Web.Api.Security;

public static class PasswordHasher
{
    #region Hash Password
    public static string HashPassword(string password)
    {
        // Return hashed password
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }
    #endregion

    #region Verify Password
    public static bool VerifyPassword(string password, string hash)
    {
        // Return verified password
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    #endregion
}
