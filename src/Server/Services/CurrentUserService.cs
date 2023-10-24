﻿using System.Security.Claims;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

namespace BlazorHero.CleanArchitecture.Server.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Claims = httpContextAccessor.HttpContext?.User.Claims.AsEnumerable()
            .Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
    }

    public List<KeyValuePair<string, string>> Claims { get; set; }

    public string UserId { get; }
}
