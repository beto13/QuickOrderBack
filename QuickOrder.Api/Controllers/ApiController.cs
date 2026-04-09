using Microsoft.AspNetCore.Mvc;
using QuickOrder.Application.Common;

namespace QuickOrder.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult ToResponse<T>(Result<T> result, string? message = null)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Value!, message));

        return MapError(result.Error);
    }

    protected IActionResult ToResponse(Result result, string? message = null)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<object>.Ok(new { }, message));

        return MapError(result.Error);
    }

    private IActionResult MapError(Error error) => error.Code switch
    {
        "NOT_FOUND" => NotFound(ApiResponse<object>.Fail(error.Message)),
        "BUSINESS_ERROR" => UnprocessableEntity(ApiResponse<object>.Fail(error.Message)),
        _ => BadRequest(ApiResponse<object>.Fail(error.Message))
    };
}
