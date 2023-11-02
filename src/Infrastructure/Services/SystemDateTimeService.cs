using CleanBlazor.Application.Abstractions.Common;

namespace CleanBlazor.Infrastructure.Services;

public class SystemDateTimeService : IDateTimeService
{
    public DateTime NowUtc => DateTime.UtcNow;
}
