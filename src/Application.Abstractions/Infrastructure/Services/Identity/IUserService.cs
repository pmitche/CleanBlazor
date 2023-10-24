using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;

public interface IUserService
{
    Task<Result<List<UserResponse>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<IResult<UserResponse>> GetAsync(string userId);

    Task<IResult> RegisterAsync(RegisterRequest request, string origin);

    Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

    Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

    Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

    Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

    Task<string> ExportToExcelAsync(string searchString = "");
}
