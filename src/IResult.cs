namespace Bens.Results;

/// <summary>
/// Base interface for API results containing business error information.
/// </summary>
public interface IResult
{
    /// <summary>
    /// 业务错误码
    /// </summary>
    int Code { get; set; }

    /// <summary>
    /// 业务错误码的简短描述
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 错误的详细说明
    /// </summary>
    string? Detail { get; set; }
}

/// <summary>
/// Generic interface for API results containing data.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public interface IResult<T> : IResult
{
    /// <summary>
    /// The data payload.
    /// </summary>
    T? Data { get; set; }
}