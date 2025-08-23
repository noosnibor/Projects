namespace Web.Api.Domain;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password, string Device);
public record AuthResponse(string AccessToken, string RefreshToken);
public record RefreshRequest(string RefreshToken, string Device);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);
public record ConfirmEmailRequest(string Token);
public record AssignRoleRequest(string UserId, string RoleName);
