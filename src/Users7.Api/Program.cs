using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Users7.Api.Constants;
using Users7.Api.Middleware;
using Users7.Api.Responses;
using Users7.Application;
using Users7.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .Distinct()
                .ToArray();

            var response = new ApiResponse<object>(
                ApiCodes.ValidationError,
                ApiMessages.InvalidRequest,
                Errors: errors,
                TraceId: context.HttpContext.TraceIdentifier);

            return new BadRequestObjectResult(response);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Users7 API",
        Version = "v1",
        Description = "API para gerenciamento de usuários."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.HasStarted || response.ContentLength > 0 || response.ContentType is not null)
    {
        return;
    }

    var statusCode = (HttpStatusCode)response.StatusCode;
    var (code, message) = statusCode switch
    {
        HttpStatusCode.NotFound => (ApiCodes.NotFound, ApiMessages.RouteNotFound),
        HttpStatusCode.MethodNotAllowed => (ApiCodes.MethodNotAllowed, ApiMessages.MethodNotAllowed),
        _ => (ApiCodes.BadRequest, ApiMessages.BadRequest)
    };

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new ApiResponse<object>(
        code,
        message,
        TraceId: statusCodeContext.HttpContext.TraceIdentifier));
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
