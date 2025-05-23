using Microsoft.Extensions.Options;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Congiguration;
using System.Net.Http.Json;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Handles authentication and token management for the CrowdStrike Falcon API.
/// </summary>
public class CrowdStrikeAuthService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeOptions _options;
    private string? _accessToken;
    private DateTime _expiresAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrowdStrikeAuthService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client instance used to make requests.</param>
    /// <param name="options">The options containing CrowdStrike credentials and endpoint base URL.</param>
    public CrowdStrikeAuthService(HttpClient httpClient, IOptions<CrowdStrikeOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
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

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/oauth2/token")
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
        _expiresAt = DateTime.UtcNow.AddSeconds(result.ExpiresIn - 60); // buffer

        return _accessToken!;
    }
}
