using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.RegularExpressions;

namespace Bens.Results.Test;

/// <summary>
/// https://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/
/// </summary>
public class ApiResultTest
{
    [Fact]
    public async Task TestBasicJsonUsage()
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
                            return new ApiResult<string>("success");
                        });
                        endpoints.MapGet("/success", () =>
                        {
                            return ApiResult.EmptySuccess;
                        });
                        endpoints.MapGet("/failure", () =>
                        {
                            return ApiResult.FuzzyFail;
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/hello");
        var context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"code\":0,\"title\":\"OK\",\"data\":\"success\"}", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await host.GetTestClient().GetAsync("/success");
        context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"code\":0,\"title\":\"OK\"}", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await host.GetTestClient().GetAsync("/failure");
        context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"code\":-1,\"title\":\"request fail\"}", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestBasicXmlUsage()
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
                            var res = new ApiResult<string>("success", 200);
                            res.ContentType = "application/xml";
                            return res;
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/hello");
        var context = await response.Content.ReadAsStringAsync();
        context = Regex.Replace(context, @"\n\s*", "").Replace("\r", "");
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-8\"?><ApiResult xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Code>0</Code><Title>OK</Title><Data>success</Data></ApiResult>", context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestBasicTextPlainUsage()
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
                        endpoints.MapGet("/plain", () =>
                        {
                            var res = new ApiResult<object>(new { A = "" }, 200);
                            res.ContentType = "text/plain";
                            return res;
                        });

                        endpoints.MapGet("/stream", () =>
                        {
                            var res = new ApiResult<object>(new { A = "" }, 200);
                            res.ContentType = "application/octet-stream";
                            return res;
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/plain");
        var context = await response.Content.ReadAsStringAsync();
        context = Regex.Replace(context, @"\n\s*", "").Replace("\r", "");
        Assert.Equal((new { A = "" }).ToString(), context);

        try
        {
            response = await host.GetTestClient().GetAsync("/stream");
            Assert.Equal("not support content type", context);
        }
        catch (InvalidOperationException e)
        {
            Assert.Equal("Unsupported content type: application/octet-stream", e.Message);
        }
    }

    [Fact]
    public async Task TestJsonOptionsInject()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddApiResult();
                    services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    });
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/hello", () =>
                        {
                            return new ApiResult<string>("success", 400);
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/hello");
        var context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"Code\":0,\"Title\":\"OK\",\"Data\":\"success\"}", context);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestUseController()
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
                        endpoints.MapControllers();
                    });
                });
        })
        .StartAsync();
        var response = await host.GetTestClient().GetAsync("/api/Test/Get");
        var context = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"code\":0,\"title\":\"OK\",\"data\":\"success\"}", context);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestApiResultOkFail()
    {
        using var host = await new HostBuilder()
        .ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddApiResult();
                    services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    });
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/ok", () =>
                        {
                            return ApiResult.Ok("success");
                        });
                        endpoints.MapGet("/fail", () =>
                        {
                            return ApiResult.Fail("error", 400);
                        });
                    });
                });
        })
        .StartAsync();

        var response = await host.GetTestClient().GetAsync("/ok");
        var context = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"Code\":0,\"Title\":\"OK\",\"Data\":\"success\"}", context);

        response = await host.GetTestClient().GetAsync("/fail");
        context = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("{\"Code\":-1,\"Title\":\"error\"}", context);
    }

    [Fact]
    public void TestApiResultProps()
    {
        var a = new ApiResult<int>(123);
        Assert.Equal(200, a.StatusCode);
        a = new ApiResult<int>(123, 200);
        Assert.Equal(123, a.Data);
        Assert.Equal("OK", a.Title);
        Assert.Equal(0, a.Code);
        Assert.Equal("application/json; charset=utf-8", a.ContentType);

        a.Title = "Err";
        Assert.Equal("Err", a.Title);

        a.Code = 1;
        Assert.Equal(1, a.Code);
    }
}