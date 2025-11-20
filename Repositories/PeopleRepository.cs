using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly PeopleDbContext _context;
        private readonly ILogger<PeopleRepository> _logger;

        public PeopleRepository(PeopleDbContext context, ILogger<PeopleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _logger.LogInformation($"{nameof(AddPerson)} of {nameof(PeopleRepository)} called");

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonById(Guid id)
        {
            _logger.LogInformation($"{nameof(DeletePersonById)} of {nameof(PeopleRepository)} called");

            IEnumerable<Person> peopleToRemove = GetAllPersonsQueryable()
                .Where(p => p.Id == id);

            _context.RemoveRange(peopleToRemove);

            int rowsDeleted = await _context.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<IEnumerable<Person>> GetAllPersons()
        {
            _logger.LogInformation($"{nameof(GetAllPersons)} of {nameof(PeopleRepository)} called");

            return await GetAllPersonsQueryable()
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Person>> SearchPeople(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation($"{nameof(SearchPeople)} of {nameof(PeopleRepository)} called");

            return await GetAllPersonsQueryable()
                .Where(predicate)
                .ToArrayAsync();
        }

        public async Task<Person?> GetPersonById(Guid id) 
        {
            _logger.LogInformation($"{nameof(GetPersonById)} of {nameof(PeopleRepository)} called");

            return await GetAllPersonsQueryable()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person?> UpdatePerson(Person person)
        {
            _logger.LogInformation($"{nameof(UpdatePerson)} of {nameof(PeopleRepository)} called");

            Person? personToUpdate = await _context.People
                .FirstOrDefaultAsync(p => p.Id == person.Id);

            if (personToUpdate == null)
            {
                return personToUpdate;
            }

            personToUpdate.Name = person.Name;
            personToUpdate.Email = person.Email;
            personToUpdate.DateOfBirth = person.DateOfBirth;
            personToUpdate.Gender = person.Gender;
            personToUpdate.CountryId = person.CountryId;
            personToUpdate.Address = person.Address;
            personToUpdate.ReceiveNewsLetters = person.ReceiveNewsLetters;

            await _context.SaveChangesAsync();

            return personToUpdate;
        }

        private IQueryable<Person> GetAllPersonsQueryable()
        {
            return _context.People
                .Include(nameof(Person.Country));
        }
    }
}
