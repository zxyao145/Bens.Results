using Bens.Results.Internal;
using Bens.Results.ResultExecutors;
using Bens.Results.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Bens.Results;

/// <summary>
/// Extension methods for configuring API result services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds API result services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddApiResult(this IServiceCollection services)
    {
        services.AddScoped<ApiResultExecutor>();
        services.AddScoped(typeof(ApiResultExecutor<>));
        services.AddScoped<IApiResultXmlSerializer, DefaultApiResultXmlSerializer>();
        services.AddScoped(typeof(IApiResultXmlSerializer<>), typeof(DefaultApiResultXmlSerializer<>));
        return services;
    }
}