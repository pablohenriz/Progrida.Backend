using Progrida.Domain.Entities;

namespace Progrida.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    /// <summary>Gera o access token (curta duração) para o usuário autenticado.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Gera um refresh token opaco (longa duração).</summary>
    string GenerateRefreshToken();
}
