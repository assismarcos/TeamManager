using TeamManagement.Application.RequestsResponse;
using TeamManagement.Application.RequestsResponse.Member;
using TeamManagement.Application.Validation.Member;
using TeamManagement.Core.Abstractions;
using TeamManagement.Core.Entities;
using TeamManagement.Core.ExternalServices;

namespace TeamManagement.Application.Services.Member;

public class MemberService(IMemberRepository repository, ICountryService countryService) : IMemberService
{
    public async Task<ApiResponse<IEnumerable<GetMemberResponse>>> GetAllAsync(string? tags)
    {
        var members = await repository.GetAllAsync(tags);

        var memberResponse = members.Select(member => new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            SalaryPerYear = member.SalaryPerYear,
            Type = member.Type,
            ContractDuration = member.ContractDuration,
            Role = member.Role,
            Tags = member.Tags,
            CountryName = member.CountryName,
            CurrencySymbol = member.CurrencySymbol,
            CurrencyName = member.CurrencyName
        }).ToList();

        return ApiResponse<IEnumerable<GetMemberResponse>>.CreateResponse(memberResponse.Count > 0 ? memberResponse : null);
    }

    public async Task<ApiResponse<GetMemberResponse>> GetAsync(int id)
    {
        var member = await repository.GetAsync(id);
        if (member == null)
        {
            return ApiResponse<GetMemberResponse>.Empty();
        }

        var response = new GetMemberResponse
        {
            Id = member.Id,
            Name = member.Name,
            SalaryPerYear = member.SalaryPerYear,
            Type = member.Type,
            ContractDuration = member.ContractDuration,
            Role = member.Role,
            Tags = member.Tags,
            CurrencyCode = member.CurrencyCode,
            CountryName = member.CountryName,
            CurrencySymbol = member.CurrencySymbol,
            CurrencyName = member.CurrencyName
        };

        return ApiResponse<GetMemberResponse>.CreateResponse(response);
    }

    public async Task<ApiResponse<int>> AddAsync(AddMemberRequest request)
    {
        var validation = await new AddMemberValidator().ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ApiResponse<int>.CreateResponse(validation.Errors);
        }

        var countryInfo = await countryService.GetCountryInfo(request.CountryName);
        if (countryInfo.CountryName == null)
        {
            return ApiResponse<int>.CreateResponse($"Country not found: {request.CountryName}");
        }

        var newMember = new Core.Entities.Member
        {
            Name = request.Name,
            SalaryPerYear = request.SalaryPerYear,
            Type = request.Type,
            ContractDuration = request.Type == MemberType.Contractor ? request.ContractDuration : null,
            Role = request.Type == MemberType.Employee ? request.Role : null,
            Tags = request.Tags,
            CountryName = countryInfo.CountryName,
            CurrencyCode = countryInfo.CurrencyCode,
            CurrencySymbol = countryInfo.CurrencySymbol,
            CurrencyName = countryInfo.CurrencyDescription
        };
        var id = await repository.AddAsync(newMember);

        return ApiResponse<int>.CreateResponse(id);
    }

    public async Task<ApiResponse<bool>> UpdateAsync(UpdateMemberRequest request)
    {
        var validation = await new UpdateMemberValidator().ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ApiResponse<bool>.CreateResponse(validation.Errors);
        }

        var countryInfo = await countryService.GetCountryInfo(request.CountryName);
        if (countryInfo.CountryName == null)
        {
            return ApiResponse<bool>.CreateResponse($"Country not found: {request.CountryName}");
        }

        var member = new Core.Entities.Member
        {
            Id = request.Id,
            Name = request.Name,
            SalaryPerYear = request.SalaryPerYear,
            Type = request.Type,

            ContractDuration = request.Type == MemberType.Contractor ? request.ContractDuration : null,
            Role = request.Type == MemberType.Employee ? request.Role : null,

            Tags = request.Tags,
            CountryName = countryInfo.CountryName,
            CurrencyCode = countryInfo.CurrencyCode,
            CurrencySymbol = countryInfo.CurrencySymbol,
            CurrencyName = countryInfo.CurrencyDescription
        };

        return ApiResponse<bool>.CreateResponse(await repository.UpdateAsync(member));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var member = await repository.GetAsync(id);
        return member == null 
            ? ApiResponse<bool>.CreateResponse($"Member not found: {id}") 
            : ApiResponse<bool>.CreateResponse(await repository.DeleteAsync(id));
    }
}