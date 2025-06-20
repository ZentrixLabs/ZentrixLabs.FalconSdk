namespace ZentrixLabs.FalconSdk.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents an error returned by the API.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}