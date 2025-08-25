using DataAccess.Library;
using System.Data;
using Web.Api.Domain;

namespace Web.Api.Data;

public class UserRepository(IConfiguration configuration)
{
    #region Data Access
    private readonly Sql _db = new() { Database = "Default", Configuration = configuration, Server = ServerOption.SqlServer }; 
    #endregion;

    #region Get user by email
    public async Task<UserModel> GetByEmail(string email)
    {
        string script = "SELECT * FROM tblUsers WHERE Email = @Email";

        List<UserModel> result =  await _db.GetAsync<UserModel, dynamic>(script, CommandType.Text, new { Email = email });

        return result.FirstOrDefault()!;
    }
    #endregion

    #region Get user by Id
    public async Task<UserModel> GetById(Guid id)
    {
        string script = "SELECT *" +
                        "FROM [tblUsers] u" +
                        "WHERE u.Id = @Id";
        List<UserModel> result =  await _db.GetAsync<UserModel, dynamic>(script, CommandType.Text, new { Id = id });

        return result.FirstOrDefault()!;
    }
    #endregion

    #region Insert User
    public async Task CreateUser(string email, string passwordHash)
    {
        // Create new user Id using a global unique identifier
        Guid id = Guid.NewGuid();

        // Generate script to insert a user
        string script = "INSERT INTO [tblUsers] (Email, PasswordHash)" +
                        "VALUES(@Email, @PasswordHash)";

        // Add user to the db
        await _db.AddAsync(script, CommandType.Text, new {Email = email, PasswordHash = passwordHash });
    }
    #endregion

    #region Confirm Email
    public async Task ConfirmEmail(Guid userId)
    {
        // Generate script to update a user
        string script = "UPDATE [tblUsers] " +
                        "SET EmailConfirmed = 1, UpdateAt = SYSUTCDATETIME() " +
                        "WHERE Id = @UserId";

        // Update user in the db
        await _db.UpdateAsync(script, CommandType.Text, new { UserId = userId });
    }
    #endregion

    #region Update Password
    public async Task UpdatePassword(Guid userId, string hash)
    {
        // Generate script to update a user
        string script = "UPDATE [tblUsers]" +
                        "SET PasswordHash = @Hash, UpdateAt = SYSUTCDATETIME()" +
                        "WHERE Id = @UserId";

        // Update user in the db
        await _db.UpdateAsync(script, CommandType.Text, new { UserId = userId, Hash = hash });
    }
    #endregion

    #region Get role name
    public async Task<IEnumerable<string>> GetRoleName(Guid userId)
    {
        // Generate script to update a user
        string script = "SELECT r.Name FROM [tblRole] r" +
                        "INNER JOIN [tblUserRoles] ur" +
                        "ON ur.RoleId = r.Id" +
                        "WHERE ur.RoleId = @UserId";

        // Update user in the db
        return await _db.GetAsync<string, dynamic>(script, CommandType.Text, new { UserId = userId });
    }
    #endregion
}
