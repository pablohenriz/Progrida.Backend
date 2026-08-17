namespace Progrida.Application.Common.Exceptions;

/// <summary>Erros de validação de entrada (antes mesmo de chegar ao Domain).</summary>
public class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("Um ou mais campos são inválidos.")
    {
        Errors = errors.ToList();
    }

    public ValidationException(string error) : this(new[] { error }) { }
}
