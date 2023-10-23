using System.Reflection;

namespace BlazorHero.CleanArchitecture.Contracts;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
