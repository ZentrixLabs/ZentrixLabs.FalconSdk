

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Services;

namespace ZentrixLabs.FalconSdk;

public static class FalconSdkServiceCollectionExtensions
{
    public static IServiceCollection AddFalconSdk(this IServiceCollection services, Action<CrowdStrikeOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHttpClient();
        services.AddSingleton<CrowdStrikeAuthService>();
        services.AddSingleton<CrowdStrikeDeviceService>();
        services.AddSingleton<CrowdStrikeSpotlightService>();
        return services;
    }

    public static IServiceCollection AddFalconSdk(this IServiceCollection services, IConfiguration configuration, string sectionName = "CrowdStrike")
    {
        services.Configure<CrowdStrikeOptions>(configuration.GetSection(sectionName));
        services.AddHttpClient();
        services.AddSingleton<CrowdStrikeAuthService>();
        services.AddSingleton<CrowdStrikeDeviceService>();
        services.AddSingleton<CrowdStrikeSpotlightService>();
        return services;
    }
}