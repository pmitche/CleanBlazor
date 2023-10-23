using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Account;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = BlazorHero.CleanArchitecture.Shared.Wrapper.IResult;

namespace BlazorHero.CleanArchitecture.Server.Controllers.Identity;

[Authorize]
[Route("api/identity/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUserService _currentUser;

    public AccountController(IAccountService accountService, ICurrentUserService currentUser)
    {
        _accountService = accountService;
        _currentUser = currentUser;
    }

    /// <summary>
    ///     Update Profile
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Status 200 OK</returns>
    [HttpPut(nameof(UpdateProfile))]
    public async Task<ActionResult> UpdateProfile(UpdateProfileRequest model)
    {
        IResult response = await _accountService.UpdateProfileAsync(model, _currentUser.UserId);
        return Ok(response);
    }

    /// <summary>
    ///     Change Password
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Status 200 OK</returns>
    [HttpPut(nameof(ChangePassword))]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
    {
        IResult response = await _accountService.ChangePasswordAsync(model, _currentUser.UserId);
        return Ok(response);
    }

    /// <summary>
    ///     Get Profile picture by Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Status 200 OK </returns>
    [HttpGet("profile-picture/{userId}")]
    [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 60)]
    public async Task<IActionResult> GetProfilePictureAsync(string userId) =>
        Ok(await _accountService.GetProfilePictureAsync(userId));

    /// <summary>
    ///     Update Profile Picture
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Status 200 OK</returns>
    [HttpPost("profile-picture/{userId}")]
    public async Task<IActionResult> UpdateProfilePictureAsync(UpdateProfilePictureRequest request) =>
        Ok(await _accountService.UpdateProfilePictureAsync(request, _currentUser.UserId));
}
