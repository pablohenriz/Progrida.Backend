using Microsoft.AspNetCore.Mvc;
using Progrida.Application.Users;

namespace Progrida.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserHandler _registerHandler;
    private readonly LoginUserHandler _loginHandler;

    public AuthController(RegisterUserHandler registerHandler, LoginUserHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterUserRequest request, CancellationToken ct)
    {
        var result = await _registerHandler.Handle(request, ct);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginUserRequest request, CancellationToken ct)
    {
        var result = await _loginHandler.Handle(request, ct);
        return Ok(result);
    }

    // POST /api/auth/refresh e /api/auth/logout ficam como próximo passo:
    // exigem uma tabela de refresh tokens (com revogação) na Infrastructure,
    // conforme o item 12 do plano de arquitetura.
}
