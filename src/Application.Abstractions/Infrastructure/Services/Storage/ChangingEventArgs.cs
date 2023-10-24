using System.Diagnostics.CodeAnalysis;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Storage;

[ExcludeFromCodeCoverage]
public class ChangingEventArgs : ChangedEventArgs
{
    public bool Cancel { get; set; }
}
