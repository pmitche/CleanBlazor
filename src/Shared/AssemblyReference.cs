using System.Reflection;

namespace BlazorHero.CleanArchitecture.Shared;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
