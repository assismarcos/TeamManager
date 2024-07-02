using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using TeamManagement.Core.ExternalServices;

namespace TeamManagement.Infra.Services;

public class CountryService(ILogger<CountryService> logger, HttpClient httpClient) : ICountryService
{
    public async Task<CountryInfo> GetCountryInfo(string countryName)
    {
        logger.LogInformation("Requesting country info to RestCountries.com");
        var response = await httpClient.GetAsync($"name/{countryName.ToLower()}");
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Country {CountryName} not found", countryName);
            return null!;
        }

        var json = await response.Content.ReadAsStringAsync();

        var countryInfo = new CountryInfo();

        var node = JsonNode.Parse(json);
        if (node == null || node.AsArray().Count <= 0)
        {
            logger.LogInformation("Result: {@result}", countryInfo);
            return countryInfo;
        }

        var firstElement = node.AsArray().First();
        if (firstElement == null)
        {
            logger.LogInformation("Result: {@result}", countryInfo);
            return countryInfo;
        }

        var currencyInfo = GetCurrencyInfo(firstElement);
        countryInfo = new CountryInfo(
            GetCountryName(firstElement),
            currencyInfo.currencyCode ?? string.Empty,
            currencyInfo.currencySymbol ?? string.Empty,
            currencyInfo.currencyDescription ?? string.Empty);

        logger.LogInformation("Result: {@result}", countryInfo);

        return countryInfo;
    }

    private string GetCountryName(JsonNode node)
    {
        logger.LogInformation("Extracting the country name");
        var nodeObj = node.AsObject().First(x => x.Key == "name").Value;
        if (nodeObj == null)
        {
            logger.LogInformation("Country not found");
            return string.Empty;
        }

        var innerNode = JsonNode.Parse(nodeObj.ToString());
        var result = innerNode?.AsObject()
            .Where(x => x.Key == "common")
            .Select(x => x.Value)
            .First()?.GetValue<string>();

        logger.LogInformation("Result: {result}", result);
        return result ?? string.Empty;
    }

    private (string? currencyCode, string? currencySymbol, string? currencyDescription) GetCurrencyInfo(JsonNode node)
    {
        logger.LogInformation("Extracting the currency information");
        var nodeObj = node.AsObject().First(x => x.Key == "currencies").Value;
        if (nodeObj == null)
        {
            logger.LogInformation("Currency info not found");
            return (string.Empty, string.Empty, string.Empty);
        }

        var currencies = JsonNode.Parse(nodeObj.ToString());
        var currencyCode = currencies?.AsObject().First().Key;
        var currencySymbol = string.Empty;
        var currencyDescription = string.Empty;

        if (string.IsNullOrEmpty(currencyCode))
        {
            logger.LogInformation(
                "Result: CurrencyCode: {CurrencyCode}, CurrencySymbol: {CurrencySymbol}, CurrencyDescription: {CurrencyDescription}",
                currencyCode, currencySymbol, currencyDescription);
            return (currencyCode, currencySymbol, currencyDescription);
        }

        currencySymbol = currencies?[currencyCode]?["symbol"]?.GetValue<string>();
        currencyDescription = currencies?[currencyCode]?["name"]?.GetValue<string>();

        logger.LogInformation(
            "Result: CurrencyCode: {CurrencyCode}, CurrencySymbol: {CurrencySymbol}, CurrencyDescription: {CurrencyDescription}",
            currencyCode, currencySymbol, currencyDescription);

        return (currencyCode, currencySymbol, currencyDescription);
    }
}