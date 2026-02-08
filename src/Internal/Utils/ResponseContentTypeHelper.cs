using Microsoft.Net.Http.Headers;
using System.Text;

namespace Bens.Results.Internal.Utils;

/// <summary>
/// Helper for resolving content type and encoding for HTTP responses.
/// Based on: https://github.com/dotnet/aspnetcore/blob/b98185b6376b966c5a051926986acaf204fe4e76/src/Shared/ResponseContentTypeHelper.cs
/// </summary>
internal static class ResponseContentTypeHelper
{
    public static void ResolveContentTypeAndEncoding(
        string? actionResultContentType,
        string? httpResponseContentType,
        (string defaultContentType, Encoding defaultEncoding) @default,
        Func<string, Encoding?> getEncoding,
        out string resolvedContentType,
        out Encoding resolvedContentTypeEncoding)
    {
        var (defaultContentType, defaultContentTypeEncoding) = @default;

        // 1. User sets the ContentType property on the action result
        if (actionResultContentType is not null)
        {
            resolvedContentType = actionResultContentType;
            resolvedContentTypeEncoding = getEncoding(actionResultContentType) ?? defaultContentTypeEncoding;
            return;
        }

        // 2. User sets the ContentType property on the http response directly
        if (!string.IsNullOrEmpty(httpResponseContentType))
        {
            resolvedContentType = httpResponseContentType;
            resolvedContentTypeEncoding = getEncoding(httpResponseContentType) ?? defaultContentTypeEncoding;
            return;
        }

        // 3. Fall-back to the default content type
        resolvedContentType = defaultContentType;
        resolvedContentTypeEncoding = defaultContentTypeEncoding;
    }

    public static Encoding? GetEncoding(string mediaType)
    {
        return MediaTypeHeaderValue.TryParse(mediaType, out var parsed)
            ? parsed.Encoding
            : null;
    }
}