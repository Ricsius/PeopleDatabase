using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries = new List<Country>();

        public CountryResponse AddCountry(CountryAddRequest? request)
        {
            if (request == null) 
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrEmpty(request.CountryName))
            {
                throw new ArgumentException(nameof(request));
            }

            bool duplicateCountry = _countries.Any(c => c.Name == request.CountryName);

            if (duplicateCountry)
            {
                throw new ArgumentException("Country name already exists");
            }

            Country country = request.ToCountry();
            country.Id = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public IEnumerable<CountryResponse> GetAllCountries() 
        {
            return _countries
                .Select(c => c.ToCountryResponse())
                .ToArray();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            CountryResponse? foundCountry = _countries
                .FirstOrDefault(c => c.Id == id)?
                .ToCountryResponse();

            return foundCountry;
        }
    }
}
