# Bens.Results

An API result wrapper library for ASP.NET Core that provides a unified response format with built-in support for HTTP status codes and business error handling.

## Features

- **Unified Response Format** - Consistent API responses with `Code`, `Title`, `Detail`, and optional `Data`
- **Multiple Content Types** - Built-in support for JSON and XML serialization
- **HTTP Status Integration** - Seamless integration with ASP.NET Core's `IResult` and `IActionResult`
- **Type Safety** - Generic and non-generic variants with full type support
- **Validation Support** - Model state validation error handling
- **Modern C#** - Leverages latest C# syntax and best practices
- **Production Ready** - Fully tested and optimized for performance

## Installation

```bash
dotnet add package Bens.Results
```

## Configuration

Register the services in your `Program.cs`:

```csharp
using Bens.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiResult();

var app = builder.Build();
app.Run();
```

## Basic Usage

### Simple Success Response

```csharp
// Returns default success (Code: 0, Title: "ok")
return ApiResult.Ok();

// Returns success with data
return ApiResult.Ok(new { Name = "John", Email = "john@example.com" });
```

### Error Responses

```csharp
// Generic failure
return ApiResult.Fail("Operation failed");

// Failure with custom code and HTTP status
return ApiResult.Fail(1001, "Payment required", 402);

// HTTP status helpers
return ApiResult.BadRequest("Invalid input");
return ApiResult.Unauthorize("Authentication required");
return ApiResult.Forbid("Access denied");
return ApiResult.ServerError("Internal error occurred");
```

### Validation Errors

```csharp
public IActionResult CreateUser(CreateUserRequest request)
{
    if (!ModelState.IsValid)
    {
        return ApiResult.BadRequest(ModelState);
    }

    // ... create user logic

    return ApiResult.Ok(user);
}
```

## Advanced Usage

### Custom Response

```csharp
public class ApiResult : ApiResultBase<ApiResult>
{
    public IActionResult CustomResponse()
    {
        return new ApiResult
        {
            Code = 2000,
            Title = "Custom message",
            Detail = "Detailed description",
            StatusCode = 200,
            ContentType = "application/json"
        };
    }
}
```

### XML Responses

```csharp
// Create XML response
return ApiResult.Xml()
    .WithStatusCode(200)
    .WithCode(0)
    .WithTitle("Success");

// XML response with data
return ApiResult.Xml<UserInfo>(userData);
```

### Response Format

#### Success Response (JSON)

```json
{
  "code": 0,
  "title": "ok",
  "statusCode": 200,
  "data": {
    "id": 123,
    "name": "John Doe"
  }
}
```

#### Error Response (JSON)

```json
{
  "code": 400000,
  "title": "One or more validation errors occurred.",
  "statusCode": 400,
  "detail": "Invalid request parameters",
  "errors": {
    "email": ["The Email field is required."],
    "age": ["The field Age must be between 18 and 120."]
  }
}
```

## API Reference

### Classes

| Class | Description |
|-------|-------------|
| `ApiResult` | Non-generic API result for responses without data |
| `ApiResult<T>` | Generic API result for responses with typed data |
| `ApiResultProblemDetails` | Extended result for validation errors |
| `ApiListResult<T>` | Result for list data pagination |

### Static Factory Methods

#### Success Methods

- `ApiResult.Ok()` - Empty success response
- `ApiResult.Ok<T>(T data)` - Success with data

#### Failure Methods

- `ApiResult.Fail(string title, int? statusCode = null)` - Generic failure
- `ApiResult.Fail(int code, string title, int statusCode = 200)` - Failure with custom code
- `ApiResult.Fail<T>(...)` - Generic failure with data type

#### HTTP Status Helpers

- `ApiResult.BadRequest(string title, int code = -1)` - 400 Bad Request
- `ApiResult.BadRequest(ModelStateDictionary, string title, int code)` - Validation errors
- `ApiResult.Unauthorize(string title, int code = -1)` - 401 Unauthorized
- `ApiResult.Forbid(string title, int code = -1)` - 403 Forbidden
- `ApiResult.ServerError(string title, int code = -1)` - 500 Internal Server Error

### Factory Creators

- `ApiResult.New()` - Create new empty result
- `ApiResult.New<T>()` - Create new generic result
- `ApiResult.Json()` - Create JSON result (default)
- `ApiResult.Xml()` - Create XML result

### Interfaces

| Interface | Description |
|-----------|-------------|
| `IResult` | Base interface with Code, Title, Detail |
| `IResult<T>` | Generic interface with Data property |
| `IApiResult` - Inherits `IResult`, `IActionResult`, `IStatusCodeHttpResult`, `IContentTypeHttpResult` |
| `IApiResult<T>` | Generic API result interface |

## Extension Methods

### WithXxx Methods

```csharp
var result = new ApiResult<UserInfo>()
    .WithCode(0)
    .WithTitle("Success")
    .WithDetail("Operation completed")
    .WithStatusCode(200)
    .WithContentType("application/json")
    .WithData(userInfo);
```

## License

This is part of the Bens framework.

## Contributing

Contributions are welcome! Please ensure all tests pass before submitting a pull request.
