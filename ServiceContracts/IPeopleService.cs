using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPeopleService
    {
        /// <summary>
        /// Adds a new person into the list of persons
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the same person details, along with a newly generated ID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? request);

        /// <summary>
        /// Updates a person in the list of persons
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the updated person details</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);

        /// <summary>
        /// Deletes a person in the list of persons
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns whether the deletion was successful or not</returns>
        Task<bool> DeletePerson(Guid? id);

        /// <summary>
        /// Return all persons
        /// </summary>
        /// <returns>Returns an IEnumerable of PersonResponse type</returns>
        Task<IEnumerable<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given id
        /// </summary>
        /// <param name="id">ID (guid) to search</param>
        /// <returns>Matching person as PersonResponse object</returns>
        Task<PersonResponse?> GetPersonById(Guid? id);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns>Returns all matching persons based on the given search field and search string</returns>
        Task<IEnumerable<PersonResponse>> SearchPeople(string? searchBy, string? searchString);

        /// <summary>
        /// Returns a sorted IEnumerable of people
        /// </summary>
        /// <param name="people"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns>Returns the sorted people as a IEnumerable<PersonResponse></returns>
        Task<IEnumerable<PersonResponse>> GetSortedPeople(IEnumerable<PersonResponse> people, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Returns the stored people in CSV format
        /// </summary>
        /// <returns>Returns a MemoryStream that contains the people's data in a CSV format</returns>
        Task<MemoryStream> GetPeopleCsv();

        /// <summary>
        /// Returns the stored people in Excel format
        /// </summary>
        /// <returns>Returns a MemoryStream that contains the people's data in Excel format</returns>
        Task<MemoryStream> GetPeopleExcel();
    }
}
