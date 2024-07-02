using TeamManagement.Core.Entities;

namespace TeamManagement.Application.RequestsResponse.Member;

public class GetMemberResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal SalaryPerYear { get; set; }
    public MemberType Type { get; set; }
    public int? ContractDuration { get; set; }
    public string? Role { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
}