using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _repository;

        public CountriesService(ICountriesRepository repository)
        {
            _repository = repository;
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

            Country? foundCountry = await _repository.GetCountryByName(request.CountryName);
            bool duplicateCountry = foundCountry != null;

            if (duplicateCountry)
            {
                throw new ArgumentException("Country name already exists");
            }

            Country country = request.ToCountry();
            country.Id = Guid.NewGuid();

            await _repository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<IEnumerable<CountryResponse>> GetAllCountries()
        {
            IEnumerable<Country> countries = await _repository.GetAllCountries();

            return countries.Select(c => c.ToCountryResponse());
        }

        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Country? foundCountry = await _repository.GetCountryById(id.Value);

            return foundCountry?.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcel(IFormFile file)
        {
            int insertedCountries = 0;
            MemoryStream stream = new MemoryStream();

            await file.CopyToAsync(stream);

            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];
                int rowCount = workSheet.Dimension.Rows;

                for (int i = 2; i <= rowCount; i++)
                {
                    string? country = workSheet.Cells[i, 1].Value.ToString();

                    if (!string.IsNullOrEmpty(country))
                    {
                        IEnumerable<CountryResponse> countries = await GetAllCountries();

                        bool duplicate = countries.Any(c => c.CountryName == country);

                        if (!duplicate)
                        {
                            CountryAddRequest request = new CountryAddRequest() 
                            {
                                CountryName = country
                            };

                            await AddCountry(request);
                            insertedCountries++;
                        }
                    }
                }
            }

            return insertedCountries;
        }
    }
}
