using System.Reflection;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
