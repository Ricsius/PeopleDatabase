using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PeopleService : IPeopleService
    {
        private readonly List<Person> _people = new List<Person>();
        private readonly ICountriesService _countriesService = new CountriesService();

        public PersonResponse AddPerson(PersonAddRequest? request)
        {
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
            throw new NotImplementedException();
        }
    }
}
