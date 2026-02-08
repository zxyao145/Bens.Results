using Bens.Results.Internal.Utils;
using Bens.Results.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Bens.Results.ResultExecutors;

internal sealed class ApiResultExecutor : IActionResultExecutor<IApiResult>
{
    private static readonly string DefaultContentType = new MediaTypeHeaderValue("application/json")
    {
        Encoding = Encoding.UTF8
    }.ToString();

    private readonly JsonOptions _jsonOptions;
    private readonly IApiResultXmlSerializer _apiResultXmlSerializer;

    public ApiResultExecutor(
        IOptions<JsonOptions> jsonOptions,
        IApiResultXmlSerializer apiResultXmlSerializer)
    {
        _jsonOptions = jsonOptions.Value;
        _apiResultXmlSerializer = apiResultXmlSerializer;
    }

    public Task ExecuteAsync(ActionContext context, IApiResult result)
    {
        ArgumentNullException.ThrowIfNull(context);
        return ExecuteAsync(context.HttpContext, result);
    }

    public async Task ExecuteAsync(HttpContext httpContext, IApiResult result)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(result);

        var response = httpContext.Response;

        ResponseContentTypeHelper.ResolveContentTypeAndEncoding(
            result.ContentType,
            response.ContentType,
            (DefaultContentType, Encoding.UTF8),
            MediaType.GetEncoding,
            out var resolvedContentType,
            out _);

        response.ContentType = resolvedContentType;

        if (result.StatusCode.HasValue)
        {
            response.StatusCode = result.StatusCode.Value;
        }

        if (resolvedContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
            resolvedContentType.Contains("text/json", StringComparison.OrdinalIgnoreCase))
        {
            await JsonSerializer.SerializeAsync(
                response.BodyWriter.AsStream(),
                result,
                result.GetType(),
                _jsonOptions.JsonSerializerOptions,
                httpContext.RequestAborted);
            return;
        }

        if (resolvedContentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
        {
            _apiResultXmlSerializer.Serialize(response.BodyWriter.AsStream(), result);
            return;
        }

        if (resolvedContentType.Contains("text/plain", StringComparison.OrdinalIgnoreCase))
        {
            await response.BodyWriter.WriteAsync(ReadOnlyMemory<byte>.Empty, httpContext.RequestAborted);
            //await using var writer = _writerFactory
            //    .CreateWriter(response.Body, resolvedContentTypeEncoding);
            //await writer.WriteAsync(result.Data?.ToString() ?? "");
            //await writer.FlushAsync();
            return;
        }

        throw new InvalidOperationException($"Unsupported content type: {resolvedContentType}");
    }
}