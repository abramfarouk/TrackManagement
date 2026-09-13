using System.Net;
using System.Text.Json;
using TrackManagement.API.Response;
using TrackManagement.Application.Exceptions;

namespace TrackManagement.Api.Middleware;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;

        switch (ex)
        {
            case NotFoundException notFound:
                context.Response.StatusCode =
                    (int)HttpStatusCode.NotFound;

                response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = notFound.Message
                };

                break;

            case ValidationAppException validation:
                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = validation.Message,
                    Errors = validation.Errors
                };

                break;

            case ConflictException conflict:
                context.Response.StatusCode =
                    (int)HttpStatusCode.Conflict;

                response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = conflict.Message
                };

                break;

            default:
                _logger.LogError(
                    ex,
                    "Unhandled exception while processing {Path}",
                    context.Request.Path);

                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                response = new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                };

                break;
        }

        var json = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }
}