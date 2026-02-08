using Bens.Results.Serializers;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Bens.Results.Internal;

internal sealed class DefaultApiResultXmlSerializer : IApiResultXmlSerializer
{
    private static readonly XmlSerializer Serializer = new(typeof(ApiResult), new XmlRootAttribute("ApiResult"));
    private readonly ILogger<DefaultApiResultXmlSerializer> _logger;

    public DefaultApiResultXmlSerializer(ILogger<DefaultApiResultXmlSerializer> logger)
    {
        _logger = logger;

    }

    public void Serialize(Stream responseStream, IApiResult result)
    {
        if (result is ApiResult apiResult)
        {
            Serializer.Serialize(responseStream, apiResult);
            return;
        }

        _logger.LogError("Cannot serialize {type} to XML.", result.GetType());
        throw new InvalidOperationException($"Cannot serialize {result.GetType()} to XML.");
    }
}