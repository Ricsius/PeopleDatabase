using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly PeopleDbContext _context;

        public CountriesRepository(PeopleDbContext context)
        {
            _context = context;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<IEnumerable<Country>> GetAllCountries()
        {
            return await _context.Countries.ToArrayAsync();
        }

        public async Task<Country?> GetCountryById(Guid id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
