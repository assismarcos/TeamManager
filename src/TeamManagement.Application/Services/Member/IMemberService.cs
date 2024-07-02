using TeamManagement.Application.RequestsResponse.Member;
using TeamManagement.Application.RequestsResponse;

namespace TeamManagement.Application.Services.Member;

public interface IMemberService
{
    Task<ApiResponse<IEnumerable<GetMemberResponse>>> GetAllAsync(string? tags);
    Task<ApiResponse<GetMemberResponse>> GetAsync(int id);
    Task<ApiResponse<int>> AddAsync(AddMemberRequest request);
    Task<ApiResponse<bool>> UpdateAsync(UpdateMemberRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}