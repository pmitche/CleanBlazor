using BlazorHero.CleanArchitecture.Application.Configurations;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHero.CleanArchitecture.Infrastructure.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeService, SystemDateTimeService>();
        services.AddTransient<IMailService, SmtpMailService>();

        services.AddOptions<MailConfiguration>().Bind(configuration.GetSection(nameof(MailConfiguration)));

        return services;
    }
}
