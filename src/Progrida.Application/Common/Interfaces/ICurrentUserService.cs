namespace Progrida.Application.Common.Interfaces;

/// <summary>
/// Abstrai "quem é o usuário autenticado". Implementado na API lendo o
/// HttpContext, mas a Application nunca sabe o que é HTTP.
/// Isso é o que garante a Regra 3 em TODOS os casos de uso:
/// "GET /api/tasks" nunca significa "me dê todas as tarefas", e sim
/// "me dê as tarefas do usuário autenticado".
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
