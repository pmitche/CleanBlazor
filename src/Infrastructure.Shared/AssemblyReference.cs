using System.Reflection;

namespace BlazorHero.CleanArchitecture.Infrastructure.Shared;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
