using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Bens.Results;

public abstract class ApiResultBase<T> : IApiResult
    where T : ApiResultBase<T>
{
    public const string DefaultTitle = "OK";
    public const int DefaultCode = 0;

#if DEBUG
    [JsonPropertyOrder(-1002)]
#endif
    public int Code { get; set; } = DefaultCode;

    /// <summary>
    /// 结果简单描述
    /// </summary>
#if DEBUG
    [JsonPropertyOrder(-1001)]
#endif
    public string Title { get; set; } = DefaultTitle;

    /// <summary>
    /// 结果详细描述
    /// </summary>
#if DEBUG
    [JsonPropertyOrder(-1000)]
#endif
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; set; }

    #region HTTP Settings

    /// <summary>
    /// HTTP Status Code，for custom the response status code
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Response ContentType
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public string? ContentType { get; set; } = "application/json; charset=utf-8";

    #endregion

    protected ApiResultBase()
    {
    }

    protected ApiResultBase(int code, string title, string? detail = null)
    {
        Code = code;
        Title = title;
        Detail = detail;
    }

    /// <summary>
    /// Creates a new ApiResultBase with custom values.
    /// </summary>
    /// <param name="title">自定义消息提示</param>
    /// <param name="detail">自定义消息描述</param>
    /// <param name="code">自定义响应码</param>
    protected ApiResultBase(string title, string? detail = null, int code = 0)
    {
        Code = code;
        Title = title;
        Detail = detail;
    }

    public virtual Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return ExecuteAsync(context.HttpContext);
    }

    public abstract Task ExecuteAsync(HttpContext httpContext);

    public T WithCode(int code)
    {
        Code = code;
        return (T)this;
    }

    public T WithTitle(string title)
    {
        Title = title;
        return (T)this;
    }

    public T WithDetail(string detail)
    {
        Detail = detail;
        return (T)this;
    }

    public T WithStatusCode(int? statusCode)
    {
        StatusCode = statusCode;
        return (T)this;
    }

    public T WithContentType(string? contentType)
    {
        ContentType = contentType;
        return (T)this;
    }
}
