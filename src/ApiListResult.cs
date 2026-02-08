using System.Diagnostics.CodeAnalysis;

namespace Bens.Results;

/// <summary>
/// API result containing a list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
[ExcludeFromCodeCoverage]
public class ApiListResult<T> : ApiResult<IList<T>>
{
    public ApiListResult(IList<T> data) : base(data)
    {
    }

    public ApiListResult(int code, string title, int? statusCode = null)
        : base(code, title, statusCode)
    {
    }

    public ApiListResult(Exception exception, int code)
        : base(exception, code)
    {
    }
}