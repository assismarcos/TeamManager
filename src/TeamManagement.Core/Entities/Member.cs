namespace TeamManagement.Core.Entities;

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal SalaryPerYear { get; set; }
    public MemberType Type { get; set; }
    public int? ContractDuration { get; set; }
    public string? Role { get; set; }
    public string Tags { get; set; }
    public string CountryName { get; set; }
    public string CurrencyCode { get; set; }
    public string CurrencySymbol { get; set; }
    public string CurrencyName { get; set; }
}