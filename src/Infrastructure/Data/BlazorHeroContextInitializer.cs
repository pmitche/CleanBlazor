using BlazorHero.CleanArchitecture.Infrastructure.Helpers;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Constants.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BlazorHero.CleanArchitecture.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<BlazorHeroContextInitializer>();
        await initialiser.InitializeAsync();
        await initialiser.SeedAsync();
    }
}

public class BlazorHeroContextInitializer
{
    private readonly BlazorHeroContext _context;
    private readonly IStringLocalizer<BlazorHeroContextInitializer> _localizer;
    private readonly ILogger<BlazorHeroContextInitializer> _logger;
    private readonly RoleManager<BlazorHeroRole> _roleManager;
    private readonly UserManager<BlazorHeroUser> _userManager;

    public BlazorHeroContextInitializer(
        UserManager<BlazorHeroUser> userManager,
        RoleManager<BlazorHeroRole> roleManager,
        BlazorHeroContext context,
        ILogger<BlazorHeroContextInitializer> logger,
        IStringLocalizer<BlazorHeroContextInitializer> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        await SeedAdministratorAsync();
        await SeedBasicUserAsync();
        await _context.SaveChangesAsync();
    }

    private async Task SeedAdministratorAsync()
    {
        //Check if Role Exists
        var adminRole = new BlazorHeroRole(RoleConstants.AdministratorRole,
            _localizer["Administrator role with full permissions"]);
        BlazorHeroRole adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
        if (adminRoleInDb == null)
        {
            await _roleManager.CreateAsync(adminRole);
            adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
            _logger.LogInformation("Seeded Administrator Role.");
        }

        //Check if User Exists
        var superUser = new BlazorHeroUser
        {
            FirstName = "Mukesh",
            LastName = "Murugan",
            Email = "mukesh@blazorhero.com",
            UserName = "mukesh",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        BlazorHeroUser superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
        if (superUserInDb == null)
        {
            await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
            IdentityResult result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
            if (result.Succeeded)
            {
                _logger.LogInformation("Seeded Default SuperAdmin User.");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }

        foreach (var permission in Permissions.GetRegisteredPermissions())
        {
            await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
        }
    }

    private async Task SeedBasicUserAsync()
    {
        //Check if Role Exists
        var basicRole =
            new BlazorHeroRole(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);
        BlazorHeroRole basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
        if (basicRoleInDb == null)
        {
            await _roleManager.CreateAsync(basicRole);
            _logger.LogInformation("Seeded Basic Role.");
        }

        //Check if User Exists
        var basicUser = new BlazorHeroUser
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@blazorhero.com",
            UserName = "johndoe",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        BlazorHeroUser basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
        if (basicUserInDb == null)
        {
            await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
            await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
            _logger.LogInformation("Seeded User with Basic Role.");
        }
    }
}
