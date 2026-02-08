using Bens.Results.ResultExecutors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Bens.Results;

/// <summary>
/// Generic API result containing data.
/// </summary>
/// <typeparam name="T">The type of data contained in the result.</typeparam>
public partial class ApiResult<T> : ApiResultBase<ApiResult<T>>, IApiResult<T>
{
    #region Properties

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    #endregion

    #region Constructors

    public ApiResult()
    {
    }

    /// <summary>
    /// Creates a new ApiResult with data.
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="statusCode">HTTP 状态码</param>
    public ApiResult(T? data = default, int statusCode = 200)
        : this(code: DefaultCode, title: DefaultTitle, statusCode: statusCode, detail: null, data: data)
    {
    }

    public ApiResult(Exception exception, int code)
        : this(code: code, title: exception.Message)
    {
    }

    /// <summary>
    /// Creates a new ApiResult with custom values.
    /// </summary>
    /// <param name="code">自定义响应码</param>
    /// <param name="title">自定义消息提示</param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <param name="detail">自定义消息详细描述</param>
    /// <param name="data">数据</param>
    public ApiResult(int code, string title, int? statusCode = null, string? detail = null, T? data = default)
        : base(code, title, detail)
    {
        Data = data;
        StatusCode = statusCode;
    }

    #endregion

    public ApiResult<T> WithData(T? data)
    {
        Data = data;
        return this;
    }

    public override Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var executor = httpContext.RequestServices.GetRequiredService<ApiResultExecutor<T>>();
        return executor.ExecuteAsync(httpContext, this);
    }

    /// <summary>
    /// Implicit conversion from non-generic ApiResult to ApiResult&lt;T&gt;.
    /// </summary>
    public static implicit operator ApiResult<T>(ApiResult r) => new()
    {
        Code = r.Code,
        Title = r.Title,
        Detail = r.Detail,
        StatusCode = r.StatusCode,
        ContentType = r.ContentType,
        Data = default
    };
}