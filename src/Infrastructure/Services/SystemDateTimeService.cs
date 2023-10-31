using BlazorHero.CleanArchitecture.Application.Abstractions.Common;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services;

public class SystemDateTimeService : IDateTimeService
{
    public DateTime NowUtc => DateTime.UtcNow;
}
