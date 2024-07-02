using TeamManagement.Application.RequestsResponse.Member;
using TeamManagement.Application.Services.Member;
using TeamManagement.Core.Entities;

namespace TeamManagement.Api.Endpoints;

public static class MemberEndpoints
{
    private static async Task<IResult> GetMembers(IMemberService memberService, string? tags = null)
    {
        var members = await memberService.GetAllAsync(tags);
        return members.Response == null
            ? Results.NotFound(members)
            : Results.Ok(members);
    }

    private static async Task<IResult> GetMember(IMemberService service, int id)
    {
        var member = await service.GetAsync(id);
        return member.Response == null
            ? Results.NotFound(member)
            : Results.Ok(member);
    }

    private static async Task<IResult> CreateMember(IMemberService memberService, AddMemberRequest request)
    {
        var memberCreation = await memberService.AddAsync(request);
        return memberCreation.Response == 0
            ? Results.BadRequest(memberCreation)
            : Results.Created(new Uri("http://localhost/api/members"), memberCreation);
    }

    private static async Task<IResult> UpdateMember(IMemberService memberService, UpdateMemberRequest request)
    {
        var response = await memberService.UpdateAsync(request);
        return !response.Response
            ? Results.BadRequest(response)
            : Results.Ok(response);
    }

    private static async Task<IResult> DeleteMember(IMemberService memberService, int id)
    {
        var response = await memberService.DeleteAsync(id);
        return !response.Response
            ? Results.NotFound(response)
            : Results.Ok(response);
    }

    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/members");

        group.MapGet("/", GetMembers)
            .WithName("GetMembers")
            .WithDescription("Get all members")
            .Produces<IEnumerable<Member>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/search", GetMembers)
            .WithDescription("Search Members")
            .Produces<IEnumerable<Member>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("{id}", GetMember)
            .WithDescription("Get a Member")
            .Produces<Member>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("", CreateMember)
            .WithDescription("Create a Member")
            .Produces<int>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status201Created)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPut("", UpdateMember)
            .WithDescription("Update a Member")
            .Produces<bool>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapDelete("{id}", DeleteMember)
            .WithDescription("Remove a Member")
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi()
            .RequireAuthorization();

        return builder;
    }
}