using System.Reflection;

namespace BlazorHero.CleanArchitecture.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
