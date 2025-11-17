using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPeopleRepository
    {
        /// <summary>
        /// Adds a new person object to the repository
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the repository</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all person objects from the repository
        /// </summary>
        /// <returns>All people stored in the repository</returns>
        Task<IEnumerable<Person>> GetAllPersons();

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching person objects</returns>
        Task<IEnumerable<Person>> SearchPeople(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Returns a person object based on the id, or null if no object has the given id
        /// </summary>
        /// <param name="id">Id to search</param>
        /// <returns>Matching person or null</returns>
        Task<Person?> GetPersonById(Guid id);

        /// <summary>
        /// Deletes a person object based on the id
        /// </summary>
        /// <param name="id">Id to search</param>
        /// <returns>Returns true, if the deletion is successful, otherwise false</returns>
        Task<bool> DeletePersonById(Guid id);

        /// <summary>
        /// Updates a person object based on the id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>The updated person object</returns>
        Task<Person?> UpdatePerson(Person person);
    }
}
