using Progrida.Application.Common.Exceptions;
using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Entities;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Users;

public record RegisterUserRequest(string Name, string Email, string Password);
public record LoginUserRequest(string Email, string Password);
public record AuthResultDto(Guid UserId, string Name, string Email, string AccessToken, string RefreshToken);

/// <summary>Caso de uso: registrar um novo usuário.</summary>
public class RegisterUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RegisterUserRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            throw new ValidationException("A senha deve ter pelo menos 8 caracteres.");

        var existing = await _users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new ValidationException("Já existe um usuário cadastrado com este email.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await _users.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new AuthResultDto(user.Id, user.Name, user.Email, accessToken, refreshToken);
    }
}

/// <summary>Caso de uso: autenticar um usuário existente.</summary>
public class LoginUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginUserHandler(IUserRepository users, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResultDto> Handle(LoginUserRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        // Mensagem genérica de propósito: nunca revelar se o email existe ou não.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new ValidationException("Email ou senha inválidos.");

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new AuthResultDto(user.Id, user.Name, user.Email, accessToken, refreshToken);
    }
}
