using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Users;

public interface IUserManager : IManager
{
    Task<Result<List<UserResponse>>> GetAllAsync();

    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

    Task<Result<UserResponse>> GetAsync(string userId);

    Task<Result<UserRolesResponse>> GetRolesAsync(string userId);

    Task<Result> RegisterUserAsync(RegisterRequest request);

    Task<Result> ToggleUserStatusAsync(ToggleUserStatusRequest request);

    Task<Result> UpdateRolesAsync(UpdateUserRolesRequest request);

    Task<string> ExportToExcelAsync(string searchString = "");
}
