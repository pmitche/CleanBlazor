using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BlazorHero.CleanArchitecture.Server.Extensions
{
    internal static class HostBuilderExtensions
    {
        internal static IHostBuilder UseSerilog(this IHostBuilder builder)
        {
	        var envvars = Environment.GetEnvironmentVariables();
	        builder.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
            return builder;
        }
    }
}
