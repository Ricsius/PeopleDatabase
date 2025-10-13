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

        public PeopleService(bool initialize = true) 
        {
            if (initialize)
            {
                Person[] peopleToAdd = new Person[]
                {
                    new Person()
                    {
                        Id = Guid.Parse("15C978DC-EFD5-45C5-82AD-9DBD53C42B2D"),
                        Name = "Haleigh",
                        Email = "hmackaile0@dailymail.co.uk",
                        DateOfBirth = DateTime.Parse("1998-04-13"),
                        Gender = "Male",
                        Address = "58 Erie Road",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("1F116DD4-2B51-4B6D-BA47-948D66B25411")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("0D0F8E63-8F26-48BA-844C-7D96F7D6A093"),
                        Name = "Vlad",
                        Email = "vcheal1@ovh.net",
                        DateOfBirth = DateTime.Parse("1991-06-15"),
                        Gender = "Other",
                        Address = "048 Dawn Street",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("38920DC5-11CC-412D-88FA-A25350F9E515")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("3D1E56DD-D28D-4F48-91B2-D671CF9762DC"),
                        Name = "Felicia",
                        Email = "fphear2@wikia.com",
                        DateOfBirth = DateTime.Parse("1994-03-27"),
                        Gender = "Female",
                        Address = "16302 Arizona Plaza",
                        ReceiveNewsLetters = true,
                        CountryId = Guid.Parse("38920DC5-11CC-412D-88FA-A25350F9E515")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("39CB661D-1808-4FDF-97F0-358C163545CB"),
                        Name = "Stella",
                        Email = "sgoomes3@loc.gov",
                        DateOfBirth = DateTime.Parse("1990-06-03"),
                        Gender = "Female",
                        Address = "5802 Butternut Place",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("38920DC5-11CC-412D-88FA-A25350F9E515")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("16ED9FED-2929-41B9-9C8C-D2005A9A4878"),
                        Name = "Carrie",
                        Email = "clindenbaum4@princeton.edu",
                        DateOfBirth = DateTime.Parse("1998-01-10"),
                        Gender = "Female",
                        Address = "56 Esch Court",
                        ReceiveNewsLetters = true,
                        CountryId = Guid.Parse("26922E6D-7EA2-4451-8171-54BA632C5FB4")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("37008868-B42A-4EDC-96E5-72C6AB3CBF10"),
                        Name = "Theodore",
                        Email = "tgradon5@phoca.cz",
                        DateOfBirth = DateTime.Parse("1996-09-26"),
                        Gender = "Male",
                        Address = "06540 Hoffman Lane",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("26922E6D-7EA2-4451-8171-54BA632C5FB4")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("95F475CD-9420-4E21-9A7F-D4850B003022"),
                        Name = "Cam",
                        Email = "ctytler6@example.com",
                        DateOfBirth = DateTime.Parse("1990-08-05"),
                        Gender = "Male",
                        Address = "3 Lakewood Junction",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("2838A2DB-C484-4414-B80C-D1E82F631D07")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("2E6838F5-B65B-4E4B-BB6C-4427820C025B"),
                        Name = "Chris",
                        Email = "cfawbert7@myspace.com",
                        DateOfBirth = DateTime.Parse("1994-03-18"),
                        Gender = "Male",
                        Address = "3436 Mosinee Hill",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("2838A2DB-C484-4414-B80C-D1E82F631D07")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("47C625C6-6C99-4C95-864D-AE0BF20200BE"),
                        Name = "Marie-jeanne",
                        Email = "mcrookall8@blinklist.com",
                        DateOfBirth = DateTime.Parse("1993-11-14"),
                        Gender = "Female",
                        Address = "70 Tony Circle",
                        ReceiveNewsLetters = true,
                        CountryId = Guid.Parse("2838A2DB-C484-4414-B80C-D1E82F631D07")
                    },
                    new Person()
                    {
                        Id = Guid.Parse("2729ED42-CF33-4212-B615-5C5A5E4EE7B6"),
                        Name = "Caitlin",
                        Email = "cswancott9@zdnet.com",
                        DateOfBirth = DateTime.Parse("1996-06-01"),
                        Gender = "Female",
                        Address = "619 Glendale Circle",
                        ReceiveNewsLetters = false,
                        CountryId = Guid.Parse("2838A2DB-C484-4414-B80C-D1E82F631D07")
                    },
                };

                _people.AddRange(peopleToAdd);
            }
        }

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

        public PersonResponse UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person? person = _people.FirstOrDefault(p => p.Id == request.PersonId);

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

            return person.ToPersonResponse();
        }

        public bool DeletePerson(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Person? personToDelete = _people.FirstOrDefault(p => p.Id == id);

            if (personToDelete == null)
            {
                return false;
            }

            _people.RemoveAll(p => p.Id == id);

            return true;
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

        private PersonResponse ConvertPersonIntoResponse(Person person)
        {
            PersonResponse response = person.ToPersonResponse();
            response.CountryName = _countriesService.GetCountryById(response.CountryId)?.CountryName;

            return response;
        }
    }
}
