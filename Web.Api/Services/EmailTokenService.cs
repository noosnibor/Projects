using System.Security.Cryptography;

namespace Web.Api.Services;

public class EmailTokenService
{
	#region Generate Email Token
	public string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)); 
	#endregion
}
