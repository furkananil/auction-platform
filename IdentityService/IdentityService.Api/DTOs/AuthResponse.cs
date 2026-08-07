namespace IdentityService.Api.DTOs;

public record AuthResponse(Guid UserId, string Email, string Token);