using LastManagement.Api.Constants;
using LastManagement.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Global.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return controller.BadRequest(new ProblemDetails
        {
            Type = ProblemDetailsConstants.Types.BAD_REQUEST,
            Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
            Status = StatusCodes.Status400BadRequest,
            Detail = result.Error
        });
    }

    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return controller.BadRequest(new ProblemDetails
        {
            Type = ProblemDetailsConstants.Types.BAD_REQUEST,
            Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
            Status = StatusCodes.Status400BadRequest,
            Detail = result.Error
        });
    }
}