using FluentAssertions;
using System.Net.Http.Json;
using System.Net;
using TeamManagement.Application.RequestsResponse.Member;
using TeamManagement.Application.RequestsResponse;
using TeamManagement.IntegrationTests.Authentication;
using TeamManagement.IntegrationTests.TestData;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TeamManagement.IntegrationTests.Endpoints
{
    public class MemberEndpointTests : IClassFixture<IntegrationTestFactory>
    {
        private readonly WireMockServer _countryServiceMock;
        private readonly HttpClient _client;

        public MemberEndpointTests(IntegrationTestFactory factory)
        {
            _client = factory.CreateClientWithFakeAuthentication();
            var port = new Random().Next(1000, 10000);
            _countryServiceMock = WireMockServer.Start(new Uri($"http://localhost:{port}/v3.1/").Port);
        }

        [Fact]
        public async Task Get_Member_Should_Return_NotFound()
        {
            var response = await _client.GetAsync("api/members/100");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_Member_Should_Return_Valid_Member()
        {
            var memberRequest = CreateMember();
            ConfigureWireMock(memberRequest.CountryName);

            var response = await _client.PostAsJsonAsync("api/members", memberRequest);
            var newMember = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

            var member = await _client.GetFromJsonAsync<ApiResponse<GetMemberResponse>>($"api/members/{newMember!.Response}");

            member?.Errors.Should().BeEmpty();
            member?.Response?.Id.Should().Be(newMember.Response);
        }

        [Fact]
        public async Task Add_Member_Should_Return_Valid_Member_Id()
        {
            var memberRequest = CreateMember();
            ConfigureWireMock(memberRequest.CountryName);

            var response = await _client.PostAsJsonAsync("api/members", memberRequest);
            var newMember = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

            newMember?.Errors.Should().BeEmpty();
            newMember?.Response.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Update_Member_Should_Change_The_Member_And_Return_True()
        {
            var memberRequest = CreateMember();
            ConfigureWireMock(memberRequest.CountryName);

            var response = await _client.PostAsJsonAsync("api/members", memberRequest);
            var newMemberId = (await response.Content.ReadFromJsonAsync<ApiResponse<int>>())!.Response;

            var memberCreated = (await _client
                .GetFromJsonAsync<ApiResponse<GetMemberResponse>>($"api/members/{newMemberId}"))!.Response;

            var updateMemberRequest = new UpdateMemberRequest
            {
                Id = memberCreated!.Id,
                Name = memberCreated.Name,
                SalaryPerYear = memberCreated.SalaryPerYear,
                Type = memberCreated.Type,
                ContractDuration = memberCreated.ContractDuration,
                Role = memberCreated.Role,
                Tags = memberCreated.Tags,
                CountryName = "brasil"
            };

            updateMemberRequest.SalaryPerYear = 5000.00m;

            response = await _client.PutAsJsonAsync("api/members", updateMemberRequest);
            var updateResult = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

            var updatedMember = await _client.GetFromJsonAsync<ApiResponse<GetMemberResponse>>($"api/members/{updateMemberRequest.Id}");

            updateResult.Errors.Should().BeEmpty();
            updateResult.Response.Should().BeTrue();
            updatedMember.Response.SalaryPerYear.Should().Be(updateMemberRequest.SalaryPerYear);
        }

        [Fact]
        public async Task Delete_Member_Should_Remove_And_Return_True()
        {
            var memberRequest = CreateMember();
            ConfigureWireMock(memberRequest.CountryName);

            var response = await _client.PostAsJsonAsync("api/members", memberRequest);
            var newMemberId = (await response.Content.ReadFromJsonAsync<ApiResponse<int>>())!.Response;

            response = await _client.DeleteAsync($"api/members/{newMemberId}");
            var resultDelete = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

            resultDelete?.Errors.Should().BeEmpty();
            resultDelete?.Response.Should().BeTrue();
        }

        private AddMemberRequest CreateMember()
        {
            return MemberFaker
                .CreateAddMemberRequest("brasil", "Developer", "Backend, Project Manager, Tech lead")
                .Generate(1)
                .First();
        }

        private void ConfigureWireMock(string countryName)
        {
            _countryServiceMock
                .Given(Request.Create().WithPath($"/v3.1/name/{countryName}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(CountryFakeData.GetCountryResponse()));
        }

        public void Dispose()
        {
            _countryServiceMock.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}