using Microsoft.AspNetCore.Mvc;

namespace Bens.Results.Test;

[ApiController]
[Route("/api/Test")]
public class TestConroller : ControllerBase
{
    [HttpGet("Get")]
    public IResult Get()
    {
        return new ApiResult<string>("success", 400);
    }
}