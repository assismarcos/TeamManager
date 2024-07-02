using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TeamManagement.Application.Services;
using TeamManagement.Application.Services.Member;
using TeamManagement.Application.Services.User;

namespace TeamManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}