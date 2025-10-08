using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

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

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException("Name can't be blank");
            }

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
    }
}
