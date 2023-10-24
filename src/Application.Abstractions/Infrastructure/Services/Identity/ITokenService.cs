using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;

public interface ITokenService
{
    Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

    Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
}
