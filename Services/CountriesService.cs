using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries = new List<Country>();

        public CountriesService(bool initialize = true)
        {
            if (initialize)
            {
                Country[] mockCountries = new Country[]
                {
                    new Country()
                    {
                        Id = Guid.Parse("2838A2DB-C484-4414-B80C-D1E82F631D07"),
                        Name = "England"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("26922E6D-7EA2-4451-8171-54BA632C5FB4"),
                        Name = "Germany"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("38920DC5-11CC-412D-88FA-A25350F9E515"),
                        Name = "Japan"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("1F116DD4-2B51-4B6D-BA47-948D66B25411"),
                        Name = "Denmark"
                    }
                };

                _countries.AddRange(mockCountries);
            }
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
