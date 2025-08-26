CREATE PROCEDURE [dbo].[GetRoleIdByName]
	@pstrName VARCHAR(50)
AS
	BEGIN
		SELECT r.flngRoleKey
		FROM tblRole r
		WHERE r.fstrName = @pstrName
	END;
