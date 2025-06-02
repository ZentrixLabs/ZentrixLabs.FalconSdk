


using System.Net;

namespace ZentrixLabs.FalconSdk.Models;

/// <summary>
/// Standard response wrapper for Falcon SDK API calls.
/// </summary>
public class FalconRequestResult<T>
{
    /// <summary>
    /// Indicates whether the request was successful (2xx status).
    /// </summary>
    public bool Success => (int)StatusCode >= 200 && (int)StatusCode < 300;

    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Deserialized response body if available and successful.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Raw response body as string.
    /// </summary>
    public string? RawResponse { get; set; }

    /// <summary>
    /// Any API-level error content extracted from a successful HTTP response (optional).
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Any exception caught during the request or parsing process.
    /// </summary>
    public Exception? Exception { get; set; }
}