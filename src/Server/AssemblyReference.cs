using System.Reflection;

namespace BlazorHero.CleanArchitecture.Server;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
