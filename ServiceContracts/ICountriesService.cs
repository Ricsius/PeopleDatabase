using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="request">Country object to add</param>
        /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? request);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as IEnumerable</CountryResponse></returns>
        Task<IEnumerable<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given id
        /// </summary>
        /// <param name="id">ID (guid) to search</param>
        /// <returns>Matching country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryById(Guid? id);

        /// <summary>
        /// Uploads countries into the database from an Excel file
        /// </summary>
        /// <param name="file">The excel file to upload</param>
        /// <returns>The number of uploaded countries</returns>
        Task<int> UploadCountriesFromExcel(IFormFile file);
    }
}
