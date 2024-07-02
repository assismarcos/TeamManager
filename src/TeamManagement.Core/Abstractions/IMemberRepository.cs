using TeamManagement.Core.Entities;

namespace TeamManagement.Core.Abstractions;

public interface IMemberRepository
{
    Task<IEnumerable<Member>> GetAllAsync(string? tags);
    Task<Member?> GetAsync(int id);
    Task<int> AddAsync(Member member);
    Task<bool> UpdateAsync(Member member);
    Task<bool> DeleteAsync(int id);
}