using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        CountryResponse AddCountry(CountryAddRequest? request);
        IEnumerable<CountryResponse> GetAllCountries();
        CountryResponse? GetCountryById(Guid? id);
    }
}
