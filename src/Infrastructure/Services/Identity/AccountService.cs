﻿using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services.Identity;

public class AccountService : IAccountService
{
    private readonly IStringLocalizer<AccountService> _localizer;
    private readonly SignInManager<BlazorHeroUser> _signInManager;
    private readonly IUploadService _uploadService;
    private readonly UserManager<BlazorHeroUser> _userManager;

    public AccountService(
        UserManager<BlazorHeroUser> userManager,
        SignInManager<BlazorHeroUser> signInManager,
        IUploadService uploadService,
        IStringLocalizer<AccountService> localizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _uploadService = uploadService;
        _localizer = localizer;
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail(_localizer["User Not Found."]);
        }

        IdentityResult identityResult = await _userManager.ChangePasswordAsync(
            user,
            model.Password,
            model.NewPassword);
        List<string> errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        return identityResult.Succeeded ? Result.Ok() : Result.Fail(errors);
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest model, string userId)
    {
        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            BlazorHeroUser userWithSamePhoneNumber =
                await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
            if (userWithSamePhoneNumber != null)
            {
                return Result.Fail(string.Format(_localizer["Phone number {0} is already used."],
                    model.PhoneNumber));
            }
        }

        BlazorHeroUser userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
        if (userWithSameEmail != null && userWithSameEmail.Id != userId)
        {
            return Result.Fail(string.Format(_localizer["Email {0} is already used."], model.Email));
        }

        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail(_localizer["User Not Found."]);
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (model.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
        }

        IdentityResult identityResult = await _userManager.UpdateAsync(user);
        List<string> errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        await _signInManager.RefreshSignInAsync(user);
        return identityResult.Succeeded ? Result.Ok() : Result.Fail(errors);
    }

    public async Task<Result<string>> GetProfilePictureAsync(string userId)
    {
        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail<string>(_localizer["User Not Found"]);
        }

        return user.ProfilePictureDataUrl;
    }

    public async Task<Result<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
    {
        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail<string>(_localizer["User Not Found"]);
        }

        var filePath = _uploadService.UploadAsync(request);
        user.ProfilePictureDataUrl = filePath;
        IdentityResult identityResult = await _userManager.UpdateAsync(user);
        List<string> errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        return identityResult.Succeeded
            ? filePath
            : Result.Fail<string>(errors);
    }
}
