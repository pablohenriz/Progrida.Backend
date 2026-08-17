using System.Net;
using System.Text.Json;
using Progrida.Application.Common.Exceptions;
using Progrida.Domain.Exceptions;

namespace Progrida.API.Middleware;

/// <summary>
/// Traduz exceções do Domain/Application em respostas HTTP consistentes.
/// Os controllers ficam livres de try/catch repetido.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                ValidationException validationEx => (HttpStatusCode.BadRequest, string.Join(" ", validationEx.Errors)),
                DomainException domainEx => (HttpStatusCode.BadRequest, domainEx.Message),
                NotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
                ForbiddenAccessException forbiddenEx => (HttpStatusCode.Forbidden, forbiddenEx.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Usuário não autenticado."),
                _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro inesperado.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(ex, "Erro não tratado ao processar {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseProgridaExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
