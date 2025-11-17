using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly PeopleDbContext _context;

        public PeopleRepository(PeopleDbContext context)
        {
            _context = context;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonById(Guid id)
        {
            IEnumerable<Person> peopleToRemove = GetAllPersonsQueryable()
                .Where(p => p.Id == id);

            _context.RemoveRange(peopleToRemove);

            int rowsDeleted = await _context.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<IEnumerable<Person>> GetAllPersons()
        {
            return await GetAllPersonsQueryable()
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Person>> SearchPeople(Expression<Func<Person, bool>> predicate)
        {
            return await GetAllPersonsQueryable()
                .Where(predicate)
                .ToArrayAsync();
        }

        public async Task<Person?> GetPersonById(Guid id) 
        {
            return await GetAllPersonsQueryable()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person?> UpdatePerson(Person person)
        {
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
