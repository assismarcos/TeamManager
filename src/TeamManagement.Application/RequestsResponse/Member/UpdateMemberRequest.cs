using TeamManagement.Core.Entities;

namespace TeamManagement.Application.RequestsResponse.Member;

public class UpdateMemberRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal SalaryPerYear { get; set; }
    public MemberType Type { get; set; }
    public int? ContractDuration { get; set; }
    public string Role { get; set; }
    public string Tags { get; set; }
    public string CountryName { get; set; }
}