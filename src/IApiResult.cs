using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bens.Results;

/// <summary>
/// Interface for API results that can be executed as HTTP responses.
/// </summary>
public interface IApiResult :
    IResult,
    Microsoft.AspNetCore.Http.IResult,
    IActionResult,
    IStatusCodeHttpResult,
    IContentTypeHttpResult
{
}

/// <summary>
/// Generic interface for API results containing data.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public interface IApiResult<T> : IResult<T>, IApiResult
{
}
