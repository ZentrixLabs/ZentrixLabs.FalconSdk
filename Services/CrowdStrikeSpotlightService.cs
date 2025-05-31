using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ZentrixLabs.FalconSdk.Configuration;
using ZentrixLabs.FalconSdk.Models;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Provides methods to interact with CrowdStrike Spotlight API for retrieving vulnerability data.
/// </summary>
public class CrowdStrikeSpotlightService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeAuthService _authService;
    private readonly CrowdStrikeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrowdStrikeSpotlightService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for API requests.</param>
    /// <param name="authService">The authentication service for obtaining access tokens.</param>
    /// <param name="options">The configuration options for CrowdStrike API.</param>
    public CrowdStrikeSpotlightService(
        HttpClient httpClient,
        CrowdStrikeAuthService authService,
        IOptions<CrowdStrikeOptions> options)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
    }

    /// <summary>
    /// Retrieves a list of vulnerability IDs for a specific host.
    /// </summary>
    /// <param name="aid">The host ID (AID) to query vulnerabilities for.</param>
    /// <returns>A list of vulnerability IDs.</returns>
    public async Task<List<string>> GetVulnerabilityIdsForHostAsync(string aid)
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"{_options.BaseUrl}/spotlight/queries/vulnerabilities/v1?filter=aid:'{aid}'";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VulnerabilityQueryResponse>();
        return result?.Resources ?? new List<string>();
    }

    /// <summary>
    /// Retrieves detailed information about vulnerabilities based on their IDs.
    /// </summary>
    /// <param name="aid">The host ID (AID) to query vulnerabilities for.</param>
    /// <param name="vulnIds">A list of vulnerability IDs to retrieve details for.</param>
    /// <param name="useHostSpecific">Indicates whether to use host-specific vulnerability IDs prefixed with the host ID (AID).</param>
    /// <param name="filter">An optional filter string for querying vulnerabilities.</param>
    /// <param name="useFacets">Indicates whether to include facets like remediation or evaluation logic in the query.</param>
    /// <returns>A list of <see cref="VulnerabilityDetail"/> objects containing detailed information about the vulnerabilities.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<List<VulnerabilityDetail>> GetVulnerabilityDetailsAsync(
        string aid,
        List<string>? vulnIds = null,
        bool useHostSpecific = true,
        string? filter = null,
        bool useFacets = false)
    {
        vulnIds ??= new List<string>();

        if (string.IsNullOrEmpty(filter))
        {
            // Automatically build the filter from the AID like the PowerShell module does
            filter = $"aid:'{aid}'";
        }

        Console.WriteLine($"🔗 Querying vulnerabilities with filter: {filter}");

        var accessToken = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"{_options.BaseUrl}/spotlight/combined/vulnerabilities/v1?filter={Uri.EscapeDataString(filter)}";
        if (useFacets)
        {
            url += "&facet=remediation,evaluation_logic";
        }

        var response = await _httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"🔎 Raw Response: {responseBody}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VulnerabilityDetailEnvelope>();
        return result?.Resources ?? new List<VulnerabilityDetail>();
    }











}
