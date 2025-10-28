using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PeopleDbContext _database;

        public CountriesService(PeopleDbContext database)
        {
            _database = database;
        }

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

            bool duplicateCountry = _database.Countries
                .Any(c => c.Name == request.CountryName);

            if (duplicateCountry)
            {
                throw new ArgumentException("Country name already exists");
            }

            Country country = request.ToCountry();
            country.Id = Guid.NewGuid();

            _database.Countries.Add(country);
            _database.SaveChanges();

            return country.ToCountryResponse();
        }

        public IEnumerable<CountryResponse> GetAllCountries()
        {
            return _database.Countries
                .Select(c => c.ToCountryResponse())
                .ToArray();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            CountryResponse? foundCountry = _database.Countries
                .FirstOrDefault(c => c.Id == id)?
                .ToCountryResponse();

            return foundCountry;
        }
    }
}
