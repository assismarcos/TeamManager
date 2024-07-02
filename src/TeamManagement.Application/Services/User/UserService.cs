using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TeamManagement.Application.RequestsResponse;
using TeamManagement.Application.RequestsResponse.User;
using TeamManagement.Application.Validation.User;
using TeamManagement.Core;
using TeamManagement.Core.Abstractions;

namespace TeamManagement.Application.Services.User;

public class UserService(IUserRepository repository, IOptions<AppSettings> appSettings) : IUserService
{
    public async Task<ApiResponse<GetUserResponse>> GetByUserNameAsync(string userName)
    {
        var user = await repository.GetByUserNameAsync(userName);
        if (user == null)
        {
            return ApiResponse<GetUserResponse>.Empty();
        }

        var response = new GetUserResponse
        {
            Id = user.Id,
            UserName = userName,
        };

        return ApiResponse<GetUserResponse>.CreateResponse(response);
    }

    public async Task<ApiResponse<int>> AddOrUpdateAsync(AddUserRequest request)
    {
        var validation = await new AddUserValidator().ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ApiResponse<int>.CreateResponse(validation.Errors);
        }

        var entity = new Core.Entities.User
        {
            Id = request.Id,
            UserName = request.UserName,
            Password = request.Password // TODO create hash
        };

        if (entity.Id == 0)
        {
            return ApiResponse<int>.CreateResponse(await repository.AddAsync(entity));
        }

        await repository.UpdateAsync(entity);
        return ApiResponse<int>.CreateResponse(entity.Id);
    }

    public async Task<ApiResponse<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest request)
    {
        var user = await repository.GetByUserNameAsync(request.UserName);

        if (user == null) 
            return ApiResponse<AuthenticateResponse>.CreateResponse("User name not found");

        var token = await GenerateToken(user);
        var response = new AuthenticateResponse()
        {
            Id = user.Id,
            UserName = user.UserName,
            Token = token
        };

        return ApiResponse<AuthenticateResponse>.CreateResponse(response);
    }

    private async Task<string> GenerateToken(Core.Entities.User user)
    {
       var tokenHandler = new JwtSecurityTokenHandler();
        var token = await Task.Run(() =>
        {
            var key = Encoding.ASCII.GetBytes(appSettings.Value.JwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.CreateToken(tokenDescriptor);
        });

        return tokenHandler.WriteToken(token);
    }
}