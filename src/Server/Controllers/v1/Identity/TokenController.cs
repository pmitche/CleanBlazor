using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.Identity;

[Route("api/v1/identity/token")]
public class TokenController : BaseApiController
{
    private readonly ITokenService _identityService;

    public TokenController(ITokenService identityService) =>
        _identityService = identityService;

    /// <summary>
    ///     Get Token (Email, Password)
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Status 200 OK</returns>
    [HttpPost]
    public async Task<ActionResult> Get(TokenRequest model)
    {
        Result<TokenResponse> response = await _identityService.LoginAsync(model);
        return Ok(response);
    }

    /// <summary>
    ///     Refresh Token
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Status 200 OK</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        Result<TokenResponse> response = await _identityService.GetRefreshTokenAsync(model);
        return Ok(response);
    }
}
