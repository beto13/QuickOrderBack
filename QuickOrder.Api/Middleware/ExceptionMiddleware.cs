using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using QuickOrder.Application.Common;
using QuickOrder.Domain.Exceptions;

namespace QuickOrder.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        ApiResponse<object> response;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                var errors = validationEx.Errors.Select(e => e.ErrorMessage);
                response = ApiResponse<object>.Fail("Errores de validación.", errors);
                logger.LogWarning("Validación fallida en {Method} {Path}: {Errors}",
                    context.Request.Method, context.Request.Path, errors);
                break;

            case BusinessException:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                response = ApiResponse<object>.Fail(exception.Message);
                logger.LogWarning("Regla de negocio violada en {Method} {Path}: {Message}",
                    context.Request.Method, context.Request.Path, exception.Message);
                break;

            case KeyNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                response = ApiResponse<object>.Fail(exception.Message);
                logger.LogWarning("{Method} {Path}: {Message}",
                    context.Request.Method, context.Request.Path, exception.Message);
                break;

            case ArgumentException:
                statusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.Fail(exception.Message);
                logger.LogWarning("{Method} {Path}: {Message}",
                    context.Request.Method, context.Request.Path, exception.Message);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                response = ApiResponse<object>.Fail("Ocurrió un error inesperado.");
                logger.LogError(exception, "Error no controlado en {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                break;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
