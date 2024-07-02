using TeamManagement.Application.RequestsResponse;
using TeamManagement.Application.RequestsResponse.User;

namespace TeamManagement.Application.Services.User;

public interface IUserService
{
    Task<ApiResponse<GetUserResponse>> GetByUserNameAsync(string userName);
    Task<ApiResponse<int>> AddOrUpdateAsync(AddUserRequest request);
    Task<ApiResponse<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest request);
}