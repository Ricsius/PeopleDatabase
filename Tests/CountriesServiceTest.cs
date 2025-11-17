using Entities;
using FluentAssertions;
using RepositoryContracts;
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
            ICountriesRepository mockRepository = TestHelper.CreateMockCountriesRepository(_countries);
            
            _countriesService = new CountriesService(mockRepository);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();     
        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = null
            };

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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

            response.CountryId.Should().NotBe(Guid.Empty);
            countries.Should().Contain(response);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public async Task GetAllCountries_Empty() 
        {
            IEnumerable<CountryResponse> countries = await _countriesService.GetAllCountries();

            countries.Should().BeEmpty();
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

            countries.Should().BeEquivalentTo(expectedCountries);
        }

        #endregion

        #region GetCountryById

        [Fact]
        public async Task GetCountryByCountryId_NullId() 
        {
            Func<Task> action = async () =>
            {
                await _countriesService.GetCountryById(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
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

            foundCountry.Should().Be(response);
        }

        #endregion
    }
}
