using Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
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
            await _database.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<IEnumerable<CountryResponse>> GetAllCountries()
        {
            return await _database.Countries
                .Select(c => c.ToCountryResponse())
                .ToArrayAsync();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Country? foundCountry = await _database.Countries
                .FirstOrDefaultAsync(c => c.Id == id);

            return foundCountry?.ToCountryResponse();
        }
    }
}
