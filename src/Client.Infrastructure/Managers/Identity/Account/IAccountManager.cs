using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Identity.Account;

public interface IAccountManager : IManager
{
    Task<Result> ChangePasswordAsync(ChangePasswordRequest model);

    Task<Result> UpdateProfileAsync(UpdateProfileRequest model);

    Task<Result<string>> GetProfilePictureAsync(string userId);

    Task<Result<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
}
