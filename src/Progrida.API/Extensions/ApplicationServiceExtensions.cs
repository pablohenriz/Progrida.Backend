using Progrida.Application.Common.Interfaces;
using Progrida.Application.Sections.Commands;
using Progrida.Application.Sections.Queries;
using Progrida.Application.Tasks.Commands;
using Progrida.Application.Tasks.Queries;
using Progrida.Application.Users;

namespace Progrida.API.Extensions;

public static class ApplicationServiceExtensions
{
    /// <summary>Registra todos os casos de uso (handlers) da Application na injeção de dependência.</summary>
    public static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
    {
        // Tasks
        services.AddScoped<GetTasksHandler>();
        services.AddScoped<GetTaskByIdHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<CompleteTaskHandler>();
        services.AddScoped<ReopenTaskHandler>();
        services.AddScoped<ReorderTasksHandler>();

        // Sections
        services.AddScoped<GetSectionsHandler>();
        services.AddScoped<CreateSectionHandler>();
        services.AddScoped<UpdateSectionHandler>();
        services.AddScoped<DeleteSectionHandler>();
        services.AddScoped<ReorderSectionsHandler>();

        // Users / Auth
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginUserHandler>();

        // Traduz HttpContext -> "usuário autenticado" para toda a Application
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
