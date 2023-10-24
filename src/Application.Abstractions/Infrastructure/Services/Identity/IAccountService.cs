using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;

public interface IAccountService
{
    Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

    Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

    Task<IResult<string>> GetProfilePictureAsync(string userId);

    Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
}
