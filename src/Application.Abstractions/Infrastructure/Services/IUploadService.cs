using BlazorHero.CleanArchitecture.Contracts;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

public interface IUploadService
{
    string UploadAsync(UploadRequest request);
}
