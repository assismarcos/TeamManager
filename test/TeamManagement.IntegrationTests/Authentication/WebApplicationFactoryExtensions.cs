using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TeamManagement.IntegrationTests.Authentication;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateClientWithFakeAuthentication<T>(this WebApplicationFactory<T> factory)
        where T : class
    {
        var client = factory
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: FakeSchemeSettings.Name);
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Test");

        return client;
    }
}