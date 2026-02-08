using Bens.Results.Serializers;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Bens.Results.Internal;

internal sealed class DefaultApiResultXmlSerializer<T> : IApiResultXmlSerializer<T>
{
    private static readonly bool IsDictionary = typeof(System.Collections.IDictionary)
        .IsAssignableFrom(typeof(T));
    private static readonly XmlSerializer? Serializer = IsDictionary
        ? null
        : new XmlSerializer(typeof(ApiResult<T>), new XmlRootAttribute("ApiResult"));

    private readonly ILogger<DefaultApiResultXmlSerializer<T>> _logger;

    public DefaultApiResultXmlSerializer(ILogger<DefaultApiResultXmlSerializer<T>> logger)
    {
        _logger = logger;
    }

    public void Serialize(Stream responseStream, IApiResult<T> result)
    {
        if (IsDictionary)
        {
            _logger.LogError("Cannot serialize IApiResult<dictionary> types to XML.");
            throw new InvalidOperationException("Cannot serialize IApiResult<IDictionary> types to XML.");
        }

        if (result is ApiResult<T> apiResult)
        {
            Serializer!.Serialize(responseStream, apiResult);
            return;
        }

        _logger.LogError("Cannot serialize {type} to XML.", result.GetType());
        throw new InvalidOperationException($"Cannot serialize {result.GetType()} to XML.");
    }
}

