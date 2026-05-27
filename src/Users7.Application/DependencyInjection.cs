using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Users7.Application.DTOs;
using Users7.Application.Interfaces;
using Users7.Application.Services;
using Users7.Application.Validators;

namespace Users7.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<UserQueryParameters>, UserQueryParametersValidator>();

        return services;
    }
}
