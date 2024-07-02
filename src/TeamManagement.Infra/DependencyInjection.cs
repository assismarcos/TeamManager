using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TeamManagement.Core.Abstractions;
using TeamManagement.Core.ExternalServices;
using TeamManagement.Infra.DataContext;
using TeamManagement.Infra.Repositories;
using TeamManagement.Infra.Services;

namespace TeamManagement.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDataContext>(_ => new TeamManagerContext(new SqlConnection(connectionString)));
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ICountryService, CountryService>();

        return services;
    }
}