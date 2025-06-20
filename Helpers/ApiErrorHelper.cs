using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ZentrixLabs.FalconSdk.Helpers;

public static class ApiErrorHelper
{
    /// <summary>
    /// Logs and checks for CrowdStrike API errors in the response body.
    /// </summary>
    public static bool LogAndCheckForApiErrors(ILogger logger, string responseBody, string apiName)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
            {
                logger.LogError("API error(s) from {Api}: {Errors}", apiName, errors.ToString());
                return true;
            }
        }
        catch
        {
            // Ignore parse errors
        }
        return false;
    }
}