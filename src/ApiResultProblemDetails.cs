using System.Diagnostics.CodeAnalysis;

namespace Bens.Results;

[ExcludeFromCodeCoverage]
internal sealed class ApiResultProblemDetails : ApiResult<Dictionary<string, string[]>>
{
    /// <summary>
    /// Gets or sets the validation errors.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}
