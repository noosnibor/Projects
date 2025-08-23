using DataAccess.Library;
using System.Data;

namespace Web.Api.Data;

public class RoleRepository(IConfiguration configuration)
{
    #region Data Access
    private readonly Sql _db = new() { Database = "Default", Configuration = configuration, Server = ServerOption.SqlServer };
    #endregion;

    #region Get role id by Name
    public async Task<Guid> GetRoleIdByName(string name)
    {
        // Generate script to select a user Id
        string script = "SELECT Id" +
                        "FROM [tblRole] r" +
                        "WHERE r.Name = @Name";

        // Retrieve the result from the db
        List<Guid> result =  await _db.GetAsync<Guid, dynamic>(script, CommandType.Text, new { Name = name });

        // Return the results
        return result.FirstOrDefault();
    }
    #endregion

    #region Assign user roles
    public async Task AssignRoles(Guid userId, Guid roleId)
    {
        // Generate script to select a user Id
        string script = "IF NOT EXISTS (SELECT 1 FROM [tblUserRoles] WHERE UserId = @UserId AND RoleId = @RoleId)" +
                        "INSERT INTO tblUserRoles (UserId, RoleId)" +
                        "VALUES (@UserId, @RoleId)";

        // Insert data in user role db
        await _db.AddAsync(script, CommandType.Text, new { UserId = userId, RoleId = roleId });
    }
    #endregion
}
