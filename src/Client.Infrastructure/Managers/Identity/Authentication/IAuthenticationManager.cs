﻿using System.Security.Claims;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Authentication;

public interface IAuthenticationManager : IManager
{
    Task<IResult> Login(TokenRequest model);

    Task<IResult> Logout();

    Task<string> RefreshToken();

    Task<string> TryRefreshToken();

    Task<string> TryForceRefreshToken();

    Task<ClaimsPrincipal> CurrentUser();
}
