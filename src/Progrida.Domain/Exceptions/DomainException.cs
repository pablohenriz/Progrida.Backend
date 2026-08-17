namespace Progrida.Domain.Exceptions;

/// <summary>
/// Thrown whenever a business rule of the Domain is broken.
/// The API layer translates this into an HTTP 400.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>
/// Thrown whenever a user tries to access/modify a resource that does not
/// belong to them. The API layer translates this into an HTTP 403.
/// This is the exception that enforces Rule 3 (ownership) across the app.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Você não tem permissão para acessar este recurso.")
        : base(message) { }
}

/// <summary>
/// Thrown whenever a requested entity does not exist.
/// The API layer translates this into an HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} \"{key}\" não foi encontrado(a).") { }
}
