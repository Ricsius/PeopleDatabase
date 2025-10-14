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
        PersonResponse AddPerson(PersonAddRequest? request);

        /// <summary>
        /// Updates a person in the list of persons
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the updated person details</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? request);

        /// <summary>
        /// Deletes a person in the list of persons
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns whether the deletion was successful or not</returns>
        bool DeletePerson(Guid? id);

        /// <summary>
        /// Return all persons
        /// </summary>
        /// <returns>Returns an IEnumerable of PersonResponse type</returns>
        IEnumerable<PersonResponse> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given id
        /// </summary>
        /// <param name="id">ID (guid) to search</param>
        /// <returns>Matching person as PersonResponse object</returns>
        PersonResponse? GetPersonById(Guid? id);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns>Returns all matching persons based on the given search field and search string</returns>
        IEnumerable<PersonResponse> SearchPeople(string? searchBy, string? searchString);

        /// <summary>
        /// Returns a sorted IEnumerable of people
        /// </summary>
        /// <param name="people"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns>Returns the sorted people as a IEnumerable<PersonResponse></returns>
        IEnumerable<PersonResponse> GetSortedPeople(IEnumerable<PersonResponse> people, string sortBy, SortOrderOptions sortOrder);
    }
}
