namespace TeamManagement.Core.ExternalServices;

public interface ICountryService
{
    Task<CountryInfo> GetCountryInfo(string countryName);
}