namespace Progrida.Application.Common.Interfaces;

/// <summary>Implementado na Infrastructure (ex.: BCrypt). A Application só conhece o contrato.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
