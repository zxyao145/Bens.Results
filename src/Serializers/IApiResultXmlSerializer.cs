namespace Bens.Results.Serializers;


public interface IApiResultXmlSerializer
{
    void Serialize(Stream responseStream, IApiResult result);
}