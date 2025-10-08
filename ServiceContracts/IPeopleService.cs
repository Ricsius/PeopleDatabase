using ServiceContracts.DTO;
using System.Collections.Generic;

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
        /// Return all persons
        /// </summary>
        /// <returns>Returns an IEnumerable of PersonResponse type</returns>
        IEnumerable<PersonResponse> GetAllPersons();
    }
}
