using System.Net;
using System.Text.Json;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
        
        _logger.LogError(exception, "An unhandled exception occurred. TraceId: {TraceId}", traceId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = traceId,
            Error = GetErrorType(exception),
            Message = GetErrorMessage(exception)
        };

        response.StatusCode = GetStatusCode(exception);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(json);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorType(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => "NotFound",
            UnauthorizedAccessException => "Unauthorized",
            InvalidOperationException => "BadRequest",
            ArgumentException => "BadRequest",
            _ => "InternalServerError"
        };
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => exception.Message,
            UnauthorizedAccessException => exception.Message,
            InvalidOperationException => exception.Message,
            ArgumentException => exception.Message,
            _ => "An error occurred while processing your request."
        };
    }
}

