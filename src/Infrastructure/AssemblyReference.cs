using System.Reflection;

namespace BlazorHero.CleanArchitecture.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
