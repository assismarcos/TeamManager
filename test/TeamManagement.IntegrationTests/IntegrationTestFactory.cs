using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TeamManagement.Api;
using TeamManagement.Infra.DataContext;
using TeamManagement.IntegrationTests.Authentication;

namespace TeamManagement.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    // docker container connection
    private const string DefaultConnectionString = "Data Source=localhost,1433;User ID=sa;Password=123Change!!;MultipleActiveResultSets=True;TrustServerCertificate=true";
    
    // localDB connection
    //private const string DefaultConnectionString = "Data Source=(LocalDb)\MSSQLLocalDB;Trusted_Connection=True";
    
    private readonly WebApplicationFactory<IApiMarker> _factory;
    private readonly string _databaseName = $"dbtest{DateTime.Now.ToFileTimeUtc()}";

    public IntegrationTestFactory()
    {
        _factory = new WebApplicationFactory<IApiMarker>().WithWebHostBuilder(builder =>
        {
            builder
                .ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IDataContext));
                    if (descriptor is not null)
                    {
                        services.Remove(descriptor);
                    }

                    EnsureCreateDatabase();

                    services.AddScoped<IDataContext>(_ => new TeamManagerContext(
                        new SqlConnection($"{DefaultConnectionString};Initial Catalog={_databaseName}")));
                    
                     services
                         .AddAuthentication(defaultScheme: FakeSchemeSettings.Name)
                         .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(FakeSchemeSettings.Name, _ => { });
                    
                    //services
                    //    .AddAuthentication(defaultScheme: "Test")
                    //    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                });
        });
    }

    public async Task InitializeAsync()
    {
        await EnsureCreateTablesAsync();
    }

    public async Task DisposeAsync()
    {
        await EnsureDeleteAsync();
    }
    
    private void EnsureCreateDatabase()
    {
        using var conn = new SqlConnection(DefaultConnectionString);
        using var cmd = new SqlCommand($"create database {_databaseName}", conn);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    private async Task EnsureCreateTablesAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDataContext>();
        
        var query = $"""
                     USE [{_databaseName}]

                     CREATE TABLE dbo.Member (
                       [Id]               [int] IDENTITY(1,1) NOT NULL,
                       [Name]             [varchar](100) NOT NULL,
                       [SalaryPerYear]    [decimal](18, 2) NOT NULL,
                       [Type]             [int] NULL,
                       [ContractDuration] [int] NULL,
                       [Role]             [varchar](100) NULL,
                       [Tags]             [varchar](200) NULL,
                       [CountryName]      [varchar](100) NULL,
                       [CurrencyCode]     [varchar](10) NULL,
                       [CurrencySymbol]   [varchar](10) NULL,
                       [CurrencyName]     [varchar](100) NULL,
                         CONSTRAINT [PK_Member] PRIMARY KEY ([Id])
                     )
                     
                     CREATE TABLE dbo.[User] (
                     	[Id]               [int] IDENTITY(1,1) NOT NULL,
                     	[UserName]         [varchar](100) NOT NULL,
                         [Password]         [varchar](150) NOT NULL,
                         CONSTRAINT [PK_User] PRIMARY KEY ([Id])
                     )
                     
                     CREATE UNIQUE INDEX IDX_UserName on dbo.[User] (UserName)
                     """;
        
        await using var cmd = new SqlCommand(query, context.Connection);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task EnsureDeleteAsync()
    {
        await using var conn = new SqlConnection(DefaultConnectionString);
        await using var cmd = new SqlCommand();
        cmd.CommandText = $"""
                           use master;
                           alter database {_databaseName} set single_user with rollback immediate
                           drop database {_databaseName};
                           """;
        cmd.Connection = conn;
        conn.Open();
        await cmd.ExecuteNonQueryAsync();
    }
}