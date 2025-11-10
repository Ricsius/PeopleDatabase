using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;

namespace Services
{
    public class PeopleService : IPeopleService
    {
        private readonly PeopleDbContext _database;

        public PeopleService(PeopleDbContext database) 
        {
            _database = database;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.Id = Guid.NewGuid();

            //_database.Sp_InsertPerson(person);
            _database.People.Add(person);
            await _database.SaveChangesAsync();

            PersonResponse response = person.ToPersonResponse();

            return response;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person? person = await _database.People
                .FirstOrDefaultAsync(p => p.Id == request.PersonId);

            if (person == null)
            {
                throw new ArgumentException("Given person ID doesn't exist");
            }

            person.Name = request.Name;
            person.Email = request.Email;
            person.DateOfBirth = request.DateOfBirth;
            person.Gender = request.Gender.ToString();
            person.CountryId = request.CountryId;
            person.Address = request.Address;
            person.ReceiveNewsLetters = request.ReceiveNewsLetters;

            await _database.SaveChangesAsync();

            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Person? personToDelete = await _database.People
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personToDelete == null)
            {
                return false;
            }

            _database.People.Remove(personToDelete);
            await _database.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PersonResponse>> GetAllPersons()
        {
            //Person[] people = _database.Sp_GetAllPeople();
            IEnumerable<Person> people = await _database.People
                .Include(nameof(Person.Country))
                .ToArrayAsync();

            return people
                .Select(p => p.ToPersonResponse())
                .ToArray();
        }

        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Person? person = await _database.People
                .Include(nameof(Person.Country))
                .FirstOrDefaultAsync(p => p.Id == id);
            PersonResponse? response = person != null 
                ? person.ToPersonResponse()
                : null;

            return response;
        }

        public async Task<IEnumerable<PersonResponse>> SearchPeople(string? searchBy, string? searchString)
        {
            IEnumerable<PersonResponse> people = await GetAllPersons();
            IEnumerable<PersonResponse> matchingPeople;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return people;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.Name):
                    matchingPeople = people
                        .Where(p => !string.IsNullOrEmpty(p.Name)
                        ? p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true);
                    break;

                case nameof(PersonResponse.Email):
                    matchingPeople = people
                        .Where(p => !string.IsNullOrEmpty(p.Email)
                        ? p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true);
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPeople = people
                        .Where(p => p.DateOfBirth.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase));
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPeople = people
                        .Where(p => !string.IsNullOrEmpty(p.Gender)
                        ? p.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase)
                        : true);
                    break;

                case nameof(PersonResponse.CountryName):
                    matchingPeople = people
                        .Where(p => !string.IsNullOrEmpty(p.CountryName)
                        ? p.CountryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true);
                    break;

                case nameof(PersonResponse.Address):
                    matchingPeople = people
                        .Where(p => !string.IsNullOrEmpty(p.Address)
                        ? p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        : true);
                    break;

                default:
                    matchingPeople = people;
                    break;
            }

            return matchingPeople;
        }

        public async Task<IEnumerable<PersonResponse>> GetSortedPeople(IEnumerable<PersonResponse> people, string sortBy, SortOrderOptions sortOrder)
        {
            IEnumerable<PersonResponse> sortedPeople;

            if (string.IsNullOrEmpty(sortBy))
            {
                return people;
            }

            switch (sortBy)
            {
                case nameof(PersonResponse.Name):
                    sortedPeople = people.OrderBy(p => p.Name);
                    break;

                case nameof(PersonResponse.Email):
                    sortedPeople = people.OrderBy(p => p.Email);
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    sortedPeople = people.OrderBy(p => p.DateOfBirth);
                    break;

                case nameof(PersonResponse.Gender):
                    sortedPeople = people.OrderBy(p => p.Gender); ;
                    break;

                case nameof(PersonResponse.CountryName):
                    sortedPeople = people.OrderBy(p => p.CountryName);
                    break;

                case nameof(PersonResponse.Address):
                    sortedPeople = people.OrderBy(p => p.Address);
                    break;

                default:
                    sortedPeople = people;
                    break;
            }

            if (sortOrder == SortOrderOptions.Descending)
            {
                sortedPeople = sortedPeople.Reverse();
            }
            ;
            return await Task.FromResult(sortedPeople);
        }

        public async Task<MemoryStream> GetPeopleCsv()
        {
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            IEnumerable<PersonResponse> people = await GetAllPersons();
            MemoryStream stream = new MemoryStream();
            CsvConfiguration csvConfiguration = new CsvConfiguration(cultureInfo);

            using (StreamWriter streamWriter = new StreamWriter(stream, leaveOpen: true))
            using (CsvWriter csvWriter = new CsvWriter(streamWriter, configuration: csvConfiguration, leaveOpen: true))
            {
                csvWriter.WriteField(nameof(PersonResponse.Name));
                csvWriter.WriteField(nameof(PersonResponse.Email));
                csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                csvWriter.WriteField(nameof(PersonResponse.Age));
                csvWriter.WriteField(nameof(PersonResponse.Gender));
                csvWriter.WriteField(nameof(PersonResponse.CountryName));
                csvWriter.WriteField(nameof(PersonResponse.Address));
                csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
                csvWriter.NextRecord();
                csvWriter.Flush();

                foreach (PersonResponse person in people)
                {
                    csvWriter.WriteField(person.Name);
                    csvWriter.WriteField(person.Email);
                    csvWriter.WriteField(person.DateOfBirth.ToString("yyyy-MM-dd"));
                    csvWriter.WriteField(person.Age);
                    csvWriter.WriteField(person.Gender);
                    csvWriter.WriteField(person.CountryName);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.ReceiveNewsLetters);
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                }
            }

            stream.Position = 0;

            return stream;
        }
    }
}
