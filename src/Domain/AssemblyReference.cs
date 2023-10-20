using System.Reflection;

namespace BlazorHero.CleanArchitecture.Domain;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
