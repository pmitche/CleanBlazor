using System.Reflection;

namespace BlazorHero.CleanArchitecture.Client;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
