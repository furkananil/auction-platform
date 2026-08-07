using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.DTOs;

public record LoginRequest(
    [property: Required, EmailAddress] string Email, [property: Required] string Password);