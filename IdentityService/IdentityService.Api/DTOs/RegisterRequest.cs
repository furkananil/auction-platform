using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.DTOs;

public record RegisterRequest(
    [Required, EmailAddress] string Email, [Required, MinLength(6)] string Password);