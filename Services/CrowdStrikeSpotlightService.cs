using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ZentrixLabs.FalconSdk.Configuration;
using ZentrixLabs.FalconSdk.Models;

namespace ZentrixLabs.FalconSdk.Services;

public class CrowdStrikeSpotlightService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeAuthService _authService;
    private readonly CrowdStrikeOptions _options;

    public CrowdStrikeSpotlightService(
        HttpClient httpClient,
        CrowdStrikeAuthService authService,
        IOptions<CrowdStrikeOptions> options)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
    }

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

    public async Task<List<VulnerabilityDetail>> GetVulnerabilityDetailsAsync(List<string> vulnIds)
    {
        if (vulnIds == null || vulnIds.Count == 0)
        {
            return new List<VulnerabilityDetail>();
        }

        var accessToken = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var idParams = string.Join(",", vulnIds);
        var url = $"{_options.BaseUrl}/spotlight/entities/vulnerabilities/v1?ids={idParams}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VulnerabilityDetailEnvelope>();
        return result?.Resources ?? new List<VulnerabilityDetail>();
    }


}
