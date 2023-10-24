namespace BlazorHero.CleanArchitecture.Application.Abstractions.Common;

public interface IDateTimeService
{
    DateTime NowUtc { get; }
}
