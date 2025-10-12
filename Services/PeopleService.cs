using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PeopleService : IPeopleService
    {
        private readonly List<Person> _people = new List<Person>();
        private readonly ICountriesService _countriesService = new CountriesService();

        public PersonResponse AddPerson(PersonAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.Id = Guid.NewGuid();

            _people.Add(person);

            PersonResponse response = ConvertPersonIntoResponse(person);

            return response;
        }

        private PersonResponse ConvertPersonIntoResponse(Person person) 
        {
            PersonResponse response = person.ToPersonResponse();
            response.CountryName = _countriesService.GetCountryById(response.CountryId)?.CountryName;

            return response;
        }

        public IEnumerable<PersonResponse> GetAllPersons()
        {
            return _people
                .Select(p => p.ToPersonResponse())
                .ToArray();
        }

        public PersonResponse? GetPersonById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            PersonResponse? response = _people
                .FirstOrDefault(p => p.Id == id)?
                .ToPersonResponse();

            return response;
        }

        public IEnumerable<PersonResponse> SearchPeople(string searchBy, string? searchString)
        {
            IEnumerable<PersonResponse> people = GetAllPersons();
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
                        ? p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)
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

        public IEnumerable<PersonResponse> GetSortedPeople(IEnumerable<PersonResponse> people, string sortBy, SortOrderOptions sortOrder)
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

            return sortedPeople;
        }
    }
}
