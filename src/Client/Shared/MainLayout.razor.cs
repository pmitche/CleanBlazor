﻿using BlazorHero.CleanArchitecture.Client.Configuration;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Shared;

public partial class MainLayout
{
    private MudTheme _currentTheme;
    private bool _rightToLeft;

    private async Task RightToLeftToggle(bool value)
    {
        _rightToLeft = value;
        await Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        _currentTheme = BlazorHeroTheme.DefaultTheme;
        _currentTheme = await ClientPreferenceManager.GetCurrentThemeAsync();
        _rightToLeft = await ClientPreferenceManager.IsRtl();
    }

    private async Task DarkMode()
    {
        var isDarkMode = await ClientPreferenceManager.ToggleDarkModeAsync();
        _currentTheme = isDarkMode
            ? BlazorHeroTheme.DefaultTheme
            : BlazorHeroTheme.DarkTheme;
    }
}
