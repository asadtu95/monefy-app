using System.Text.Json;
using Monefy.Api.DTOs.Common;
using Monefy.Api.Helpers;

namespace Monefy.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, message) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ValidationFailedException => (StatusCodes.Status400BadRequest, ex.Message),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Something went wrong.")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var payload = ApiResponse<object>.Fail(message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}
