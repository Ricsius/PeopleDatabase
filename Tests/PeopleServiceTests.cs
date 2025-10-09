using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;

namespace Tests
{
    public class PeopleServiceTests
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;
        private readonly PersonAddRequest[] _validPersonAddRequests;
        private readonly ITestOutputHelper _outputHelper;

        public PeopleServiceTests(ITestOutputHelper outputHelper)
        {
            _peopleService = new PeopleService();
            _countriesService = new CountriesService();
            _outputHelper = outputHelper;

            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "Germany"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "England"
            };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryRequest2);

            _validPersonAddRequests = new PersonAddRequest[]
            {
                new PersonAddRequest()
                {
                    Name = "Joe",
                    Email = "dummy@example.com",
                    Address = "Sample street",
                    Gender = GenderOptions.Male,
                    CountryId = countryResponse1.CountryId,
                    DateOfBirth = DateTime.Parse("2001-01-01"),
                    ReceiveNewsLetters = true,
                },
                new PersonAddRequest()
                {
                    Name = "Smith",
                    Email = "something@example.com",
                    Gender = GenderOptions.Female,
                    CountryId = countryResponse2.CountryId,
                    Address = "SomeAddress",
                    DateOfBirth = DateTime.Parse("2002-02-02")
                },
                new PersonAddRequest()
                {
                    Name = "Joseph",
                    Email = "josh@example.com",
                    Gender = GenderOptions.Other,
                    CountryId = countryResponse2.CountryId,
                    Address = "DummyAddress",
                    DateOfBirth = DateTime.Parse("2012-04-04")
                },

            };
        }

        #region AddPerson

        [Fact]
        public void AddPerson_NullPerson()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _peopleService.AddPerson(null);
            });
        }

        [Fact]
        public void AddPerson_NullName()
        {
            PersonAddRequest request = new PersonAddRequest()
            {
                Name = null
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _peopleService.AddPerson(request);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            PersonAddRequest request = _validPersonAddRequests.First();
            PersonResponse response = _peopleService.AddPerson(request);
            IEnumerable<PersonResponse> people = _peopleService.GetAllPersons();

            Assert.True(response.PersonId != Guid.Empty);
            Assert.Contains(response, people);
        }

        #endregion

        #region GetPersonById

        [Fact]
        public void GetPersonById_NullId() 
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                _peopleService.GetPersonById(null);
            });
        }

        [Fact]
        public void GetPersonById_ValidId() 
        {
            PersonAddRequest personRequest = _validPersonAddRequests[0];
            PersonResponse? responseFromAdd = _peopleService.AddPerson(personRequest);
            PersonResponse? responseFromGet = _peopleService.GetPersonById(responseFromAdd.PersonId);

            Assert.Equal(responseFromAdd, responseFromGet);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public void GetAllPersons_Empty() 
        {
            IEnumerable<PersonResponse> people = _peopleService.GetAllPersons(); 

            Assert.Empty(people);
        }

        [Fact]
        public void GetAllPersons_AddSomePeople()
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(personRequest);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd);

            IEnumerable<PersonResponse> people = _peopleService.GetAllPersons();

            PrintActualElements(people);

            foreach (PersonResponse person in peopleFromAdd)
            {
                Assert.Contains(person, people);
            }
        }

        #endregion

        #region SearchPeople

        [Fact]
        public void SearchPeople_EmptySearchText() 
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd);

            IEnumerable<PersonResponse> people = _peopleService.SearchPeople((nameof(Person.Name)), "");

            PrintActualElements(people);

            foreach (PersonResponse person in peopleFromAdd)
            {
                Assert.Contains(person, people);
            }
        }

        [Fact]
        public void SearchPeople_SomeSearchText()
        {
            string searchText = "jo";

            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd);

            IEnumerable<PersonResponse> people = _peopleService.SearchPeople((nameof(Person.Name)), searchText);

            PrintActualElements(people);

            foreach (PersonResponse person in peopleFromAdd)
            {
                if (person.Name!.Contains(searchText, StringComparison.OrdinalIgnoreCase)) 
                {
                    Assert.Contains(person, people);
                }
            }
        }

        #endregion

        #region Helpers

        private void PrintExpectedElements(IEnumerable<object> expectedElements)
        {
            _outputHelper.WriteLine("Expected:");

            foreach (object element in expectedElements)
            {
                _outputHelper.WriteLine(element.ToString());
            }
        }

        private void PrintActualElements(IEnumerable<object> actualElements)
        {
            _outputHelper.WriteLine("Actual:");

            foreach (object element in actualElements)
            {
                _outputHelper.WriteLine(element.ToString());
            }
        }

        #endregion
    }
}
