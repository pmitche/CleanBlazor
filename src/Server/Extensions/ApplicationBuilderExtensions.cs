using System.Globalization;
using BlazorHero.CleanArchitecture.Application.Configurations;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Server.Hubs;
using BlazorHero.CleanArchitecture.Server.Middlewares;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Localization;
using Microsoft.AspNetCore.Localization;

namespace BlazorHero.CleanArchitecture.Server.Extensions;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseExceptionHandling(
        this IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }

        return app;
    }

    internal static IApplicationBuilder UseForwarding(this IApplicationBuilder app, IConfiguration configuration)
    {
        AppConfiguration config = GetApplicationSettings(configuration);
        if (config.BehindSslProxy)
        {
            app.UseCors();
            app.UseForwardedHeaders();
        }

        return app;
    }

    internal static void ConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
        });
    }

    internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        => app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
            endpoints.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);
        });

    internal static IApplicationBuilder UseRequestLocalizationByCulture(this IApplicationBuilder app)
    {
        CultureInfo[] supportedCultures =
            LocalizationConstants.SupportedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();
        app.UseRequestLocalization(options =>
        {
            options.SupportedUICultures = supportedCultures;
            options.SupportedCultures = supportedCultures;
            options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        app.UseMiddleware<RequestCultureMiddleware>();

        return app;
    }

    internal static IApplicationBuilder Initialize(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        IEnumerable<IDatabaseSeeder> initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

        foreach (IDatabaseSeeder initializer in initializers)
        {
            initializer.Initialize();
        }

        return app;
    }

    private static AppConfiguration GetApplicationSettings(IConfiguration configuration)
    {
        IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }
}
