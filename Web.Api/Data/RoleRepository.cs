using DataAccess.Library;
using System.Data;

namespace Web.Api.Data;

/// <summary>
/// A centralized storage location for all roles relating queries
/// </summary>
/// <param name="configuration">Represents a set of key/value application configuration properties.</param>

public class RoleRepository(IConfiguration configuration)
{
    /// <summary>
    /// Data Access
    /// </summary>    
    private readonly Sql __db = new() { Database = "Default", Configuration = configuration, Server = ServerOption.SqlServer };
  

    #region Get role id by Name
    public async Task<Guid> GetRoleIdByName(string name)
    { 
        // Retrieve the result from the db
        List<Guid> result =  await __db.GetAsync<Guid, dynamic>("dbo.GetRoleIdByName", CommandType.StoredProcedure, new { pstrName = name });

        // Return the results
        return result.FirstOrDefault();
    }
    #endregion

    #region Assign user roles
    public async Task AssignRoles(Guid userId, Guid roleId)
    {
        // Insert data in user role db
        await __db.AddAsync("dbo.AssignRole", CommandType.StoredProcedure, new { plngUserKey = userId, plngRoleKey = roleId });
    }
    #endregion
}
