using BlazorHero.CleanArchitecture.Contracts;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Services;

public interface IUploadService
{
    string UploadAsync(UploadRequest request);
}
