using System.Security.Claims;
using Progrida.Application.Common.Interfaces;

namespace Progrida.API.Extensions;

/// <summary>
/// Única classe da API que sabe o que é HttpContext e o traduz para
/// algo que a Application entende (Regra 3 aplicada de ponta a ponta).
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

            if (value is null || !Guid.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId;
        }
    }
}
