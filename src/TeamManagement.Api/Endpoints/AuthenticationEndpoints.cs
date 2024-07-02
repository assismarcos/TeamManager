using TeamManagement.Application.RequestsResponse.User;
using TeamManagement.Application.Services.User;

namespace TeamManagement.Api.Endpoints;

public static class AuthenticationEndpoints
{
    private static async Task<IResult> AuthenticateAsync(IUserService service, AuthenticateRequest request)
    {
        var authentication = await service.AuthenticateAsync(request);
        return string.IsNullOrEmpty(authentication.Response.Token)
            ? Results.Unauthorized()
            : Results.Ok(authentication);
    }

    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/authentication");

        group.MapPost("", AuthenticateAsync)
            .WithDescription("User authentication")
            .Produces<AuthenticateResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .AllowAnonymous();

        return builder;
    }
}