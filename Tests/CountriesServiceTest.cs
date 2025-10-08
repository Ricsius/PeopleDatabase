using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest() 
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry

        [Fact]
        public void AddCountry_NullCountry()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                _countriesService.AddCountry(null);
            });            
        }

        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = null
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            CountryAddRequest request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            CountryAddRequest request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            CountryResponse response = _countriesService.AddCountry(request);

            IEnumerable<CountryResponse> countries = _countriesService.GetAllCountries();

            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countries);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        public void GetAllCountries_Empty() 
        {
            IEnumerable<CountryResponse> countries = _countriesService.GetAllCountries();

            Assert.Empty(countries);
        }

        [Fact]
        public void GetAllCountries_AddSomeCountries()
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
                CountryResponse response = _countriesService.AddCountry(request);

                expectedCountries.Add(response);
            }

            IEnumerable<CountryResponse> countries = _countriesService.GetAllCountries();

            foreach (CountryResponse country in countries)
            {
                Assert.Contains(country, expectedCountries);
            }
        }

        #endregion

        #region GetCountryById

        [Fact]
        public void GetCountryByCountryId_NullId() 
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.GetCountryById(null);
            });
        }

        [Fact]
        public void GetCountryByCountryId_ValidId()
        {
            CountryAddRequest request = new CountryAddRequest() 
            {
                CountryName = "Germany"
            };

            CountryResponse response = _countriesService.AddCountry(request);
            CountryResponse? foundCountry = _countriesService.GetCountryById(response.CountryId);

            Assert.Equal(response, foundCountry);
        }

        #endregion
    }
}
