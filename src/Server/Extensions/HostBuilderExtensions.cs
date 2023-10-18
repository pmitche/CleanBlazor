using Serilog;

namespace BlazorHero.CleanArchitecture.Server.Extensions;

internal static class HostBuilderExtensions
{
    internal static IHostBuilder UseSerilog(this IHostBuilder builder)
    {
        builder.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        return builder;
    }
}
