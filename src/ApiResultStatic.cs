using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.CodeAnalysis;

namespace Bens.Results;

[ExcludeFromCodeCoverage]
public partial class ApiResult
{
    public static readonly IApiResult EmptySuccess = new ApiResult();

    public static readonly IApiResult FuzzyFail = new ApiResult(-1, "request fail");

    #region Factory Methods

    public static ApiResult New() => new();

    public static ApiResult Json() => new();

    public static ApiResult Xml() => new ApiResult().WithContentType("application/xml; charset=utf-8");

    public static ApiResult<T> New<T>() => new ApiResult<T>();

    public static ApiResult<T> Json<T>() => new ApiResult<T>();

    public static ApiResult<T> Xml<T>() => new ApiResult<T>().WithContentType("application/xml; charset=utf-8");

    #endregion

    #region Success Methods

    public static ApiResult Ok() => new();

    /// <summary>
    /// Creates a successful result with data (JSON content type).
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <param name="data">The data to return.</param>
    public static ApiResult<T> Ok<T>(T? data) => new(data);

    #endregion

    #region Fail Methods

    public static ApiResult Fail(string title, int? statusCode = null)
    {
        return Fail(-1, title, statusCode ?? 200);
    }

    public static ApiResult Fail(int code, string title, int statusCode = 200)
    {
        return new ApiResult(code, title).WithStatusCode(statusCode);
    }

    public static ApiResult<T> Fail<T>(string title, int statusCode = 200)
    {
        return new ApiResult<T>(-1, title, statusCode);
    }

    public static ApiResult<T> Fail<T>(int code, string title, int statusCode = 200)
    {
        return new ApiResult<T>(code, title, statusCode);
    }

    #endregion

    #region HTTP Status Helpers

    public static ApiResult BadRequest(string title, int code = -1)
    {
        return new ApiResult(code, title).WithStatusCode(400);
    }

    public static ApiResult<Dictionary<string, string[]>> BadRequest(
        ModelStateDictionary modelState,
        string title = "One or more validation errors occurred.",
        int code = 400_000)
    {
        var errors = CreateErrorDictionary(modelState);
        return new ApiResultProblemDetails
        {
            StatusCode = 400,
            Errors = errors,
            Code = code,
            Title = title
        };
    }

    public static ApiResult Forbid(string title, int code = -1)
    {
        return new ApiResult(code, title).WithStatusCode(403);
    }

    public static ApiResult Unauthorize(string title, int code = -1)
    {
        return new ApiResult(code, title).WithStatusCode(401);
    }

    public static ApiResult ServerError(string title, int code = -1)
    {
        return new ApiResult(code, title).WithStatusCode(500);
    }

    #endregion

    #region Private Methods

    private static Dictionary<string, string[]> CreateErrorDictionary(ModelStateDictionary modelState)
    {
        ArgumentNullException.ThrowIfNull(modelState);

        var errorDictionary = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var (key, value) in modelState)
        {
            var errors = value.Errors;
            if (errors.Count == 0)
            {
                continue;
            }

            errorDictionary[key] = errors.Count == 1
                ? [GetErrorMessage(errors[0])]
                : errors.Select(GetErrorMessage).ToArray();
        }

        return errorDictionary;

        static string GetErrorMessage(ModelError error)
            => string.IsNullOrEmpty(error.ErrorMessage) ? string.Empty : error.ErrorMessage;
    }

    #endregion
}