using TeamManagement.Application.RequestsResponse.User;
using TeamManagement.Application.Services;
using TeamManagement.Application.Services.User;

namespace TeamManagement.Api.Endpoints;

public static class UserEndpoints
{
    private static async Task<IResult> GetUserAsync(IUserService service, string userName)
    {
        var member = await service.GetByUserNameAsync(userName);
        return member.Response == null
            ? Results.NotFound(member)
            : Results.Ok(member);
    }

    private static async Task<IResult> CreateOrUpdateUserAsync(IUserService userService, AddUserRequest request)
    {
        var userCreation = await userService.AddOrUpdateAsync(request);
        return userCreation.Response == 0
            ? Results.BadRequest(userCreation)
            : Results.Created(new Uri("http://localhost/api/users"), userCreation);
    }

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/users");

        group.MapGet("/", GetUserAsync)
            .WithDescription("Get an user")
            .Produces<GetUserResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("", CreateOrUpdateUserAsync)
            .WithDescription("Create or update an user")
            .Produces<int>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status201Created)
            .WithOpenApi()
            .RequireAuthorization();

        return builder;
    }
}