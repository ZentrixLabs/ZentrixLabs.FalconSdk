

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Services;
using ZentrixLabs.FalconSdk.Configuration;

namespace ZentrixLabs.FalconSdk;

public static class FalconSdkServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Falcon SDK services to the specified <see cref="IServiceCollection"/>.
    /// /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="CrowdStrikeOptions"/>.</param>
    /// /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configureOptions"/> is null.</exception>   
    /// /// <remarks>
    /// This method registers the necessary services for using the Falcon SDK, including HTTP client, authentication, device management, and vulnerability spotlight services.
    /// /// It also allows configuration of the SDK options through the provided action.
    /// </remarks>
    /// </summary>
    public static IServiceCollection AddFalconSdk(this IServiceCollection services, Action<CrowdStrikeOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHttpClient();
        services.AddSingleton<CrowdStrikeAuthService>();
        services.AddSingleton<CrowdStrikeDeviceService>();
        services.AddSingleton<CrowdStrikeSpotlightService>();
        return services;
    }

    /// <summary>
    /// Adds the Falcon SDK services to the specified <see cref="IServiceCollection"/> using configuration from a section.
    /// /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance to read options from.</param>
    /// /// <param name="sectionName">The name of the configuration section to read options from. Defaults to "CrowdStrike".</param>
    /// /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// /// <remarks>
    /// This method registers the necessary services for using the Falcon SDK, including HTTP client, authentication, device management, and vulnerability spotlight services.
    /// /// It reads the configuration options from the specified section in the provided configuration instance.
    /// /// </remarks>
    /// </summary>
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