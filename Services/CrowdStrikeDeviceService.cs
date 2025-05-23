using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ZentrixLabs.FalconSdk.Models;
using ZentrixLabs.FalconSdk.Options;

namespace ZentrixLabs.FalconSdk.Services;

/// <summary>
/// Provides methods to retrieve server device information from the CrowdStrike Falcon API.
/// </summary>
public class CrowdStrikeDeviceService
{
    private readonly HttpClient _httpClient;
    private readonly CrowdStrikeAuthService _authService;
    private readonly CrowdStrikeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrowdStrikeDeviceService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client instance configured for Falcon API access.</param>
    /// <param name="authService">The authentication service for obtaining access tokens.</param>
    /// <param name="options">The configuration options for CrowdStrike API endpoints.</param>
    public CrowdStrikeDeviceService(
        HttpClient httpClient,
        CrowdStrikeAuthService authService,
        IOptions<CrowdStrikeOptions> options)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
    }




    /// <summary>
    /// Retrieves a filtered list of server and domain controller devices from CrowdStrike Falcon.
    /// </summary>
    /// <returns>A list of <see cref="DeviceDetail"/> representing server devices.</returns>
    public async Task<List<DeviceDetail>> GetAllServerDevicesAsync()
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var allDeviceIds = new List<string>();
        int offset = 0;
        const int pageSize = 500;

        while (true)
        {
            var idResponse = await _httpClient.GetAsync($"{_options.BaseUrl}/devices/queries/devices/v1?limit={pageSize}&offset={offset}&sort=last_seen.desc");
            idResponse.EnsureSuccessStatusCode();

            var idData = await idResponse.Content.ReadFromJsonAsync<DeviceQueryResponse>();
            if (idData?.Resources == null || idData.Resources.Count == 0)
                break;

            allDeviceIds.AddRange(idData.Resources);
            offset += pageSize;
        }

        var allDevices = new List<DeviceDetail>();
        const int chunkSize = 100;

        foreach (var chunk in allDeviceIds.Chunk(chunkSize))
        {
            var idParams = string.Join("&ids=", chunk);
            var detailResponse = await _httpClient.GetAsync($"{_options.BaseUrl}/devices/entities/devices/v2?ids={idParams}");
            detailResponse.EnsureSuccessStatusCode();

            var rawJson = await detailResponse.Content.ReadAsStringAsync();
            var detailData = JsonSerializer.Deserialize<DeviceDetailEnvelope>(rawJson);

            var validTypes = new[] { "Server", "Domain Controller" };

            var filtered = detailData?.Resources
                ?.Where(d => d.ProductType == "2" || d.ProductType == "3" ||
                             validTypes.Contains(d.ProductTypeDesc?.Trim(), StringComparer.OrdinalIgnoreCase))
                .ToList() ?? new List<DeviceDetail>();

            allDevices.AddRange(filtered);
        }

        return allDevices;
    }
}
