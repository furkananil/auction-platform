namespace Shared.Contracts;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(claim, out var userId))
        {
            UserId = userId;
            IsAuthenticated = true;
        }
    }
}