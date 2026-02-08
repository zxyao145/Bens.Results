using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Bens.Results.Test;

public class FluentApiResultTest
{
    [Fact]
    public async Task TestJson()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddApiResult();
                    services.AddControllers();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/hello", () =>
                        {
                            return ApiResult.Json<string>()
                            .WithData("success");
                        });
                        endpoints.MapGet("/hello2", () =>
                        {
                            return ApiResult.Json<string>()
                            .WithData("success");
                        });
                    });
                });
        })
        .StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        var response = await host.GetTestClient().GetAsync("/hello", TestContext.Current.CancellationToken);
        var context = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("{\"code\":0,\"title\":\"OK\",\"data\":\"success\"}", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await host.GetTestClient().GetAsync("/hello2");
        context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"code\":0,\"title\":\"OK\",\"data\":\"success\"}", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestXml()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddApiResult();
                    services.AddControllers();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/hello", () =>
                        {
                            return ApiResult.Xml<string>()
                            .WithData("not fount")
                            .WithCode(-1)
                            .WithTitle("Error")
                            .WithStatusCode(404);
                        });
                        endpoints.MapGet("/hello2", () =>
                        {
                            return ApiResult.Xml<string>()
                            .WithData("not fount")
                            .WithCode(-1)
                            .WithTitle("Error")
                            .WithStatusCode(404);
                        });
                        endpoints.MapGet("/hello3", () =>
                        {
                            return new ApiResult<Dictionary<string, string[]>>()
                            {
                                Code = -1,
                                Data = new Dictionary<string, string[]>()
                                {
                                    { "name", new string[] { "name is required", "The length must be greater than 6" } },
                                    { "age", new string[] { "age is required" } }
                                },
                                Title = "Error",
                                StatusCode = 200,
                                ContentType = "application/xml; charset=utf-8"
                            };
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/hello");
        var context = await response.Content.ReadAsStringAsync();
        context = Regex.Replace(context, @"\n\s*", "").Replace("\r", "");
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?><ApiResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Code>-1</Code><Title>Error</Title><Data>not fount</Data></ApiResult>", context);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        response = await host.GetTestClient().GetAsync("/hello2");
        context = await response.Content.ReadAsStringAsync();
        context = Regex.Replace(context, @"\n\s*", "").Replace("\r", "");
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?><ApiResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Code>-1</Code><Title>Error</Title><Data>not fount</Data></ApiResult>", context);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        try
        {
            response = await host.GetTestClient().GetAsync("/hello3");
        }
        catch (Exception e)
        {
            
            Assert.True(e.Message == "Cannot serialize IApiResult<IDictionary> types to XML.");
        }
    }
}