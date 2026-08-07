using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.DTOs;

public record RegisterRequest(
    [property: Required, EmailAddress] string Email, [property: Required, MinLength(6)] string Password);