CREATE PROCEDURE [dbo].[AssignRole]
	@plngUserKey int,
	@plngRoleKey int
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION;

		IF NOT EXISTS (SELECT 1 
					   FROM tblUserRoles ur
					   WHERE ur.flngUserKey = @plngUserKey
					   AND ur.flngRoleKey = @plngRoleKey)

					   INSERT INTO tblUserRoles (flngUserKey, flngRoleKey)
					   VALUES (@plngUserKey, @plngRoleKey)

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH
END

