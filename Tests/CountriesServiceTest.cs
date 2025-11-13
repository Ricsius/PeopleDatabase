using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly List<Country> _countries = new List<Country>();

        public CountriesServiceTest() 
        {
            DbContextOptions options = new DbContextOptionsBuilder<PeopleDbContext>().Options;
            DbContextMock<PeopleDbContext> mockContext = new DbContextMock<PeopleDbContext>(options);

            mockContext.CreateDbSetMock(c => c.Countries, _countries);
            
            _countriesService = new CountriesService(mockContext.Object);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            {
                await _countriesService.AddCountry(null);
            });            
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse response = await _countriesService.AddCountry(request);

            IEnumerable<CountryResponse> countries = await _countriesService.GetAllCountries();

            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countries);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_Empty() 
        {
            IEnumerable<CountryResponse> countries = await _countriesService.GetAllCountries();

            Assert.Empty(countries);
        }

        [Fact]
        public async Task GetAllCountries_AddSomeCountries()
        {
            List<CountryResponse> expectedCountries = new List<CountryResponse>();
            CountryAddRequest[] requests = new CountryAddRequest[]
            {
                new CountryAddRequest() { CountryName = "Hungary" },
                new CountryAddRequest() { CountryName = "USA" },
                new CountryAddRequest() { CountryName = "England" },
                new CountryAddRequest() { CountryName = "Japan" },
            };

            foreach (CountryAddRequest request in requests)
            {
                CountryResponse response = await _countriesService.AddCountry(request);

                expectedCountries.Add(response);
            }

            IEnumerable<CountryResponse> countries = await _countriesService.GetAllCountries();

            foreach (CountryResponse country in countries)
            {
                Assert.Contains(country, expectedCountries);
            }
        }

        #endregion

        #region GetCountryById

        [Fact]
        public async Task GetCountryByCountryId_NullId() 
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.GetCountryById(null);
            });
        }

        [Fact]
        public async Task GetCountryByCountryId_ValidId()
        {
            CountryAddRequest request = new CountryAddRequest() 
            {
                CountryName = "Germany"
            };

            CountryResponse response = await _countriesService.AddCountry(request);
            CountryResponse? foundCountry = await _countriesService.GetCountryById(response.CountryId);

            Assert.Equal(response, foundCountry);
        }

        #endregion
    }
}
