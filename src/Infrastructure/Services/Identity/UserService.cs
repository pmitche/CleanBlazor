using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Exceptions;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Contracts.Mail;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Identity;
using BlazorHero.CleanArchitecture.Infrastructure.Specifications;
using BlazorHero.CleanArchitecture.Shared.Constants.Role;
using BlazorHero.CleanArchitecture.Shared.Models.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    private readonly RoleManager<BlazorHeroRole> _roleManager;
    private readonly UserManager<BlazorHeroUser> _userManager;

    public UserService(
        UserManager<BlazorHeroUser> userManager,
        IMapper mapper,
        RoleManager<BlazorHeroRole> roleManager,
        IMailService mailService,
        IStringLocalizer<UserService> localizer,
        IExcelService excelService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _mailService = mailService;
        _localizer = localizer;
        _excelService = excelService;
        _currentUserService = currentUserService;
    }

    public IQueryable<IUser> Users => _userManager.Users.Cast<IUser>();

    public async Task<Result<List<UserResponse>>> GetAllAsync()
    {
        List<BlazorHeroUser> users = await _userManager.Users.ToListAsync();
        var result = _mapper.Map<List<UserResponse>>(users);
        return await Result<List<UserResponse>>.SuccessAsync(result);
    }

    public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
    {
        BlazorHeroUser userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            return await Result.FailAsync(string.Format(_localizer["Username {0} is already taken."],
                request.UserName));
        }

        var user = new BlazorHeroUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.ActivateUser,
            EmailConfirmed = request.AutoConfirmEmail
        };

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            BlazorHeroUser userWithSamePhoneNumber =
                await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (userWithSamePhoneNumber != null)
            {
                return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."],
                    request.PhoneNumber));
            }
        }

        BlazorHeroUser userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
        }

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }

        await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
        if (request.AutoConfirmEmail)
        {
            return await Result<string>.SuccessAsync(user.Id,
                string.Format(_localizer["User {0} Registered."], user.UserName));
        }

        var verificationUri = await SendVerificationEmail(user, origin);
        var mailRequest = new MailRequest
        {
            From = "mail@codewithmukesh.com",
            To = user.Email,
            Body = string.Format(
                _localizer["Please confirm your account by <a href='{0}'>clicking here</a>."],
                verificationUri),
            Subject = _localizer["Confirm Registration"]
        };
        BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result<string>.SuccessAsync(user.Id,
            string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"],
                user.UserName));

    }

    public async Task<IResult<UserResponse>> GetAsync(string userId)
    {
        BlazorHeroUser user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        var result = _mapper.Map<UserResponse>(user);
        return await Result<UserResponse>.SuccessAsync(result);
    }

    public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        BlazorHeroUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
        if (isAdmin)
        {
            return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
        }

        if (user != null)
        {
            user.IsActive = request.ActivateUser;
            await _userManager.UpdateAsync(user);
        }

        return await Result.SuccessAsync();
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleModel>();
        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        List<BlazorHeroRole> roles = await _roleManager.Roles.ToListAsync();

        foreach (BlazorHeroRole role in roles)
        {
            var userRolesViewModel = new UserRoleModel { RoleName = role.Name, RoleDescription = role.Description };
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        BlazorHeroUser user = await _userManager.FindByIdAsync(request.UserId);
        if (user.Email == "mukesh@blazorhero.com")
        {
            return await Result.FailAsync(_localizer["Not Allowed."]);
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        List<UserRoleModel> selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

        BlazorHeroUser currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Exists(x => x.RoleName == RoleConstants.AdministratorRole);
            var userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
            if ((tryToAddAdministratorRole && !userHasAdministratorRole) ||
                (!tryToAddAdministratorRole && userHasAdministratorRole))
            {
                return await Result.FailAsync(
                    _localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
            }
        }

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
        return await Result.SuccessAsync(_localizer["Roles Updated"]);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
    {
        BlazorHeroUser user = await _userManager.FindByIdAsync(userId);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return await Result<string>.SuccessAsync(user.Id,
                string.Format(
                    _localizer[
                        "Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."],
                    user.Email));
        }

        throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        BlazorHeroUser user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return await Result.FailAsync(_localizer["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "api/v1/identity/users/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest
        {
            Body = string.Format(_localizer["Please reset your password by <a href='{0}'>clicking here</a>."],
                HtmlEncoder.Default.Encode(passwordResetUrl)),
            Subject = _localizer["Reset Password"],
            To = request.Email
        };
        BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        BlazorHeroUser user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return await Result.FailAsync(_localizer["An Error has occured!"]);
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
        {
            return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
        }

        return await Result.FailAsync(_localizer["An Error has occured!"]);
    }

    public async Task<int> GetCountAsync()
    {
        var count = await _userManager.Users.CountAsync();
        return count;
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        var userSpec = new UserFilterSpecification(searchString);
        List<BlazorHeroUser> users = await _userManager.Users
            .Specify(userSpec)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
        var result = await _excelService.ExportAsync(users,
            sheetName: _localizer["Users"],
            mappers: new Dictionary<string, Func<BlazorHeroUser, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["FirstName"], item => item.FirstName },
                { _localizer["LastName"], item => item.LastName },
                { _localizer["UserName"], item => item.UserName },
                { _localizer["Email"], item => item.Email },
                { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                { _localizer["PhoneNumber"], item => item.PhoneNumber },
                { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                { _localizer["IsActive"], item => item.IsActive },
                {
                    _localizer["CreatedOn (Local)"],
                    item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime()
                        .ToString("G", CultureInfo.CurrentCulture)
                },
                { _localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl }
            });

        return result;
    }

    private async Task<string> SendVerificationEmail(BlazorHeroUser user, string origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "api/v1/identity/users/confirm-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        return verificationUri;
    }
}
