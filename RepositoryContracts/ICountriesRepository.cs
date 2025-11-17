using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the repository
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the repository</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all country objects from the repository
        /// </summary>
        /// <returns>All countries stored in the repository</returns>
        Task<IEnumerable<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the id, or null if no object has the given id
        /// </summary>
        /// <param name="id">Id to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryById(Guid id);

        /// <summary>
        /// Returns a country object based on the name, or null if no object has the given name
        /// </summary>
        /// <param name="name">Name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByName(string name);
    }
}
