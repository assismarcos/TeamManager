using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TeamManagement.Api.Endpoints;
using TeamManagement.Application;
using TeamManagement.Core;
using TeamManagement.Core.ExternalServices;
using TeamManagement.Infra;
using TeamManagement.Infra.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var connectionString = builder.Configuration.GetConnectionString("TeamManager");
builder.Services
    .AddApplication()
    .AddPersistence(connectionString!);

builder.Services.AddHttpClient<ICountryService, CountryService>(client =>
{
    var countryServiceUrl = builder.Configuration.GetValue<string>("AppSettings:CountryServiceUrl") ?? throw new Exception("Invalid Country service URL");
    client.BaseAddress = new Uri(countryServiceUrl);
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:JwtSecretKey")!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapAuthenticationEndpoints()
    .MapMemberEndpoints()
    .MapUserEndpoints();
app.UseAuthorization();
app.UseAuthentication();

app.Run();