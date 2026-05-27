using System.Net;
using System.Text.Json;
using FluentValidation;
using Users7.Api.Constants;
using Users7.Api.Responses;
using Users7.Application.Exceptions;

namespace Users7.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteValidationResponseAsync(context, exception);
        }
        catch (ConflictException exception)
        {
            await WriteResponseAsync(context, HttpStatusCode.Conflict, ApiCodes.Conflict, exception.Message);
        }
        catch (ArgumentException exception)
        {
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ApiCodes.BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, ApiCodes.InternalServerError, ApiMessages.InternalServerError);
        }
    }

    private static Task WriteValidationResponseAsync(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>(
            ApiCodes.ValidationError,
            ApiMessages.InvalidRequest,
            Errors: exception.Errors.Select(error => error.ErrorMessage).Distinct().ToArray(),
            TraceId: context.TraceIdentifier);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string code, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>(code, message, TraceId: context.TraceIdentifier);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
