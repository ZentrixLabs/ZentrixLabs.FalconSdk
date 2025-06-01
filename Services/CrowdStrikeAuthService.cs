using System.Threading;
using Microsoft.Extensions.Options;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Configuration;
using System.Net.Http.Json;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Handles authentication and token management for the CrowdStrike Falcon API.
/// </summary>
public class CrowdStrikeAuthService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeOptions _options;
    private readonly ILogger<CrowdStrikeAuthService> _logger;
    private string? _accessToken;
    private DateTime _expiresAt;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private readonly TimeSpan _refreshBuffer = TimeSpan.FromSeconds(60); // configurable if needed

    /// <summary>
    /// Initializes a new instance of the <see cref="CrowdStrikeAuthService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client instance used to make requests.</param>
    /// <param name="options">The options containing CrowdStrike credentials and endpoint base URL.</param>
    /// <param name="logger">The logger instance for logging errors and info.</param>
    public CrowdStrikeAuthService(HttpClient httpClient, IOptions<CrowdStrikeOptions> options, ILogger<CrowdStrikeAuthService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a valid access token for the CrowdStrike Falcon API, reusing the cached token if not expired.
    /// </summary>
    /// <returns>The OAuth2 access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a token cannot be retrieved from the API.</exception>
    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
        {
            return _accessToken!;
        }

        await _tokenLock.WaitAsync();
        try
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
            {
                return _accessToken!;
            }

            var baseUrl = _options.BaseUrl?.TrimEnd('/');

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _options.ClientId },
                    { "client_secret", _options.ClientSecret }
                })
            };

            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CrowdStrikeTokenResponse>();

            if (result is null || string.IsNullOrEmpty(result.AccessToken))
            {
                throw new InvalidOperationException("Failed to retrieve access token from CrowdStrike.");
            }

            _accessToken = result.AccessToken;
            var expiresIn = result.ExpiresIn > 0 ? result.ExpiresIn : 300;
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn) - _refreshBuffer;

            return _accessToken!;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    /// <summary>
    /// Exposes the current CrowdStrikeOptions configuration.
    /// </summary>
    public CrowdStrikeOptions GetOptions()
    {
        return _options;
    }

    /// <summary>
    /// Starts a background refresh loop that attempts to refresh the token before it expires.
    /// Should be called once during application startup if proactive refresh is desired.
    /// </summary>
    public void StartBackgroundRefreshLoop(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var timeUntilRefresh = _expiresAt - DateTime.UtcNow - TimeSpan.FromSeconds(10);
                    if (timeUntilRefresh < TimeSpan.Zero)
                    {
                        // Expired or very close to expiring, refresh now
                        await GetAccessTokenAsync();
                        timeUntilRefresh = _expiresAt - DateTime.UtcNow - TimeSpan.FromSeconds(10);
                    }

                    await Task.Delay(timeUntilRefresh > TimeSpan.Zero ? timeUntilRefresh : TimeSpan.FromSeconds(60), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    // Log and continue loop
                    _logger.LogError(ex, "[CrowdStrikeAuthService] Token refresh error");
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
        }, cancellationToken);
    }
    /// <summary>
    /// Helper to register a background token refresh loop during application startup.
    /// Intended for use with DI containers that provide IHostApplicationLifetime or similar.
    /// </summary>
    /// <param name="serviceProvider">The application service provider.</param>
    /// <param name="token">An optional cancellation token (e.g., from IHostApplicationLifetime).</param>
    public static void EnableAutoRefresh(IServiceProvider serviceProvider, CancellationToken token)
    {
        var authService = serviceProvider.GetService(typeof(CrowdStrikeAuthService)) as CrowdStrikeAuthService;
        if (authService is not null)
        {
            authService.StartBackgroundRefreshLoop(token);
        }
    }
}