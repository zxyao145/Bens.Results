using Bens.Results.ResultExecutors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bens.Results;

public partial class ApiResult : ApiResultBase<ApiResult>
{
    public ApiResult()
    {
    }

    public ApiResult(int code, string title, string? detail = null)
        : base(code, title, detail)
    {
    }

    /// <summary>
    /// Creates a new ApiResult with custom values.
    /// </summary>
    /// <param name="title">自定义消息提示</param>
    /// <param name="detail">自定义消息描述</param>
    /// <param name="code">自定义响应码</param>
    public ApiResult(string title, string? detail = null, int code = 0)
        : base(title, detail, code)
    {
    }

    public override Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var executor = httpContext.RequestServices
            .GetRequiredService<ApiResultExecutor>();
        return executor.ExecuteAsync(httpContext, this);
    }
}
