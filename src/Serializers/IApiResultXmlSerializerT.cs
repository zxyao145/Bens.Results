namespace Bens.Results.Serializers;

public interface IApiResultXmlSerializer<T>
{
    void Serialize(Stream responseStream, IApiResult<T> result);
}
