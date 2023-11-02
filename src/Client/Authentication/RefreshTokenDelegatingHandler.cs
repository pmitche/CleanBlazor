using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CleanBlazor.Client.Authentication;

public class RefreshTokenDelegatingHandler : DelegatingHandler
{
    private readonly AuthenticationManager _authenticationManager;
    private readonly IStringLocalizer<RefreshTokenDelegatingHandler> _localizer;
    private readonly ILogger<RefreshTokenDelegatingHandler> _logger;
    private readonly NavigationManager _navigationManager;
    private readonly ISnackbar _snackBar;

    public RefreshTokenDelegatingHandler(
        AuthenticationManager authenticationManager,
        NavigationManager navigationManager,
        ISnackbar snackBar,
        IStringLocalizer<RefreshTokenDelegatingHandler> localizer,
        ILogger<RefreshTokenDelegatingHandler> logger)
    {
        _authenticationManager = authenticationManager;
        _navigationManager = navigationManager;
        _snackBar = snackBar;
        _localizer = localizer;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var absPath = request.RequestUri.AbsolutePath;
        if (absPath.Contains("token") || absPath.Contains("accounts"))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        try
        {
            var token = await _authenticationManager.TryRefreshToken();
            if (!string.IsNullOrEmpty(token))
            {
                _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to refresh token");
            _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
            await _authenticationManager.Logout();
            _navigationManager.NavigateTo("/");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
