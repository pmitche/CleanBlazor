using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;

public interface IAccountService
{
    Task<Result> UpdateProfileAsync(UpdateProfileRequest model, string userId);

    Task<Result> ChangePasswordAsync(ChangePasswordRequest model, string userId);

    Task<Result<string>> GetProfilePictureAsync(string userId);

    Task<Result<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
}
