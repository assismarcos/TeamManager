using Bogus;
using TeamManagement.Application.RequestsResponse.Member;
using TeamManagement.Core.Entities;

namespace TeamManagement.IntegrationTests.TestData;

public class MemberFaker
{
    public static Faker<AddMemberRequest> CreateAddMemberRequest(string countryName, string role, string tags)
    {
        return new Faker<AddMemberRequest>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.SalaryPerYear, f => f.Finance.Amount(18, 2))
            .RuleFor(x => x.Type, f => f.PickRandom(new List<MemberType> { MemberType.Employee, MemberType.Contractor }))
            .RuleFor(x => x.ContractDuration, f => f.Random.Short(1, 36))
            .RuleFor(x => x.Role, _=> role)
            .RuleFor(x => x.Tags, _ => tags)
            .RuleFor(x => x.CountryName, _ => countryName);
    }
}