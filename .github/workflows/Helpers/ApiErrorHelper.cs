using Microsoft.Extensions.Logging;
using System.Text.Json;
using ZentrixLabs.FalconSdk.Models;

namespace ZentrixLabs.FalconSdk.Helpers
{
    public static class ApiErrorHelper
    {
        /// <summary>
        /// Logs and detects API-level errors by inspecting the JSON response body.
        /// </summary>
        public static bool LogAndCheckForApiErrors(ILogger logger, string responseBody, string context = "[API Response]")
        {
            if (responseBody.Contains("\"errors\":", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("⚠️ {Context} contains API-level errors: {Body}", context, responseBody);
                return true;
            }

            return false;
        }
    }
}