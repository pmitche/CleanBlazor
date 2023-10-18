using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using BlazorHero.CleanArchitecture.Infrastructure.Helpers;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Constants.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BlazorHero.CleanArchitecture.Infrastructure;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly BlazorHeroContext _db;
    private readonly IStringLocalizer<DatabaseSeeder> _localizer;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly RoleManager<BlazorHeroRole> _roleManager;
    private readonly UserManager<BlazorHeroUser> _userManager;

    public DatabaseSeeder(
        UserManager<BlazorHeroUser> userManager,
        RoleManager<BlazorHeroRole> roleManager,
        BlazorHeroContext db,
        ILogger<DatabaseSeeder> logger,
        IStringLocalizer<DatabaseSeeder> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _logger = logger;
        _localizer = localizer;
    }

    public void Initialize()
    {
        AddAdministrator();
        AddBasicUser();
        _db.SaveChanges();
    }

    private void AddAdministrator() =>
        Task.Run(async () =>
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
                CreatedOn = DateTime.Now,
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
        }).GetAwaiter().GetResult();

    private void AddBasicUser() =>
        Task.Run(async () =>
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
                CreatedOn = DateTime.Now,
                IsActive = true
            };
            BlazorHeroUser basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
            if (basicUserInDb == null)
            {
                await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
                await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                _logger.LogInformation("Seeded User with Basic Role.");
            }
        }).GetAwaiter().GetResult();
}
