using System.Net.Http.Headers;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Toolbelt.Blazor;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Interceptors;

public class HttpInterceptorManager : IHttpInterceptorManager
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly HttpClientInterceptor _interceptor;
    private readonly IStringLocalizer<HttpInterceptorManager> _localizer;
    private readonly ILogger<HttpInterceptorManager> _logger;
    private readonly NavigationManager _navigationManager;
    private readonly ISnackbar _snackBar;

    public HttpInterceptorManager(
        HttpClientInterceptor interceptor,
        IAuthenticationManager authenticationManager,
        NavigationManager navigationManager,
        ISnackbar snackBar,
        IStringLocalizer<HttpInterceptorManager> localizer,
        ILogger<HttpInterceptorManager> logger)
    {
        _interceptor = interceptor;
        _authenticationManager = authenticationManager;
        _navigationManager = navigationManager;
        _snackBar = snackBar;
        _localizer = localizer;
        _logger = logger;
    }

    public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

    public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        if (e.Request.RequestUri == null)
        {
            return;
        }

        var absPath = e.Request.RequestUri.AbsolutePath;
        if (!absPath.Contains("token") && !absPath.Contains("accounts"))
        {
            try
            {
                var token = await _authenticationManager.TryRefreshToken();
                if (!string.IsNullOrEmpty(token))
                {
                    _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while attempting to refresh token");
                _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                await _authenticationManager.Logout();
                _navigationManager.NavigateTo("/");
            }
        }
    }

    public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
}
