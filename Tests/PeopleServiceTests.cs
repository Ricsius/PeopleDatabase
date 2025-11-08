using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace Tests
{
    public class PeopleServiceTests
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;
        private PersonAddRequest[] _validPersonAddRequests = [];
        private readonly ITestOutputHelper _outputHelper;
        private readonly List<Country> _countries = new List<Country>();
        private readonly List<Person> _people = new List<Person>();

        public PeopleServiceTests(ITestOutputHelper outputHelper)
        {
            PeopleDbContext mockContext = TestHelper.CreateMockPeopleDbContext(_countries, _people);

            _countriesService = new CountriesService(mockContext);
            _peopleService = new PeopleService(mockContext, _countriesService);
            _outputHelper = outputHelper;

            PrepareTestData().Wait();
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _peopleService.AddPerson(null);
            });
        }

        [Fact]
        public async Task AddPerson_NullName()
        {
            PersonAddRequest request = new PersonAddRequest()
            {
                Name = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _peopleService.AddPerson(request);
            });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            PersonAddRequest request = _validPersonAddRequests.First();
            PersonResponse response = await _peopleService.AddPerson(request);
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            Assert.True(response.PersonId != Guid.Empty);
            Assert.Contains(response, people);
        }

        #endregion

        #region GetPersonById

        [Fact]
        public async Task GetPersonById_NullId() 
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            {
                await _peopleService.GetPersonById(null);
            });
        }

        [Fact]
        public async Task GetPersonById_ValidId() 
        {
            PersonAddRequest personRequest = _validPersonAddRequests[0];
            PersonResponse? responseFromAdd = await _peopleService.AddPerson(personRequest);
            PersonResponse? responseFromGet = await _peopleService.GetPersonById(responseFromAdd.PersonId);

            Assert.Equal(responseFromAdd, responseFromGet);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_Empty() 
        {
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons(); 

            Assert.Empty(people);
        }

        [Fact]
        public async Task GetAllPersons_AddSomePeople()
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest personRequest in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(personRequest);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd);

            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            PrintActualElements(people);

            foreach (PersonResponse person in peopleFromAdd)
            {
                Assert.Contains(person, people);
            }
        }

        #endregion

        #region SearchPeople

        [Fact]
        public async Task SearchPeople_EmptySearchText() 
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd);

            IEnumerable<PersonResponse> people = await _peopleService.SearchPeople((nameof(PersonResponse.Name)), "");

            PrintActualElements(people);

            foreach (PersonResponse person in peopleFromAdd)
            {
                Assert.Contains(person, people);
            }
        }

        [Fact]
        public async Task SearchPeople_SomeSearchText()
        {
            string searchText = "jo";

            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PrintExpectedElements(peopleFromAdd.Where(p => p.Name!.Contains(searchText, StringComparison.OrdinalIgnoreCase)));

            IEnumerable<PersonResponse> people = await _peopleService.SearchPeople((nameof(PersonResponse.Name)), searchText);

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

        #region GetSortedPeople

        [Fact]
        public async Task GetSortedPeople_SortBy_Name_Descending()
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PersonResponse[] expectedPeople = peopleFromAdd
                .OrderByDescending(p => p.Name)
                .ToArray();

            PrintExpectedElements(expectedPeople);

            IEnumerable<PersonResponse> people = await _peopleService
                .GetSortedPeople(peopleFromAdd, (nameof(Person.Name)), SortOrderOptions.Descending);
            PersonResponse[] peopleArray = people.ToArray();

            PrintActualElements(peopleArray);

            for (int i = 0; i < expectedPeople.Length; i++)
            {
                Assert.Equal(expectedPeople[i], peopleArray[i]);
            }
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_Null_Request()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            {
                await _peopleService.UpdatePerson(null);
            });
        }

        [Fact]
        public async Task UpdatePerson_InvalidId()
        {
            PersonUpdateRequest request = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _peopleService.UpdatePerson(request);
            });
        }

        [Fact]
        public async Task UpdatePerson_Null_PersonName()
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(addRequest);
            }

            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();
            PersonUpdateRequest updateRequest = people
                .First()
                .ToPersonUpdateRequest();
            updateRequest.Name = null;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _peopleService.UpdatePerson(updateRequest);
            });
        }

        [Fact]
        public async Task UpdatePerson_Update_Name_Email()
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(addRequest);
            }

            string updatedName = "UpdatedName";
            string updatedEmail = "UpdatedEmailAddress@example.com";
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();
            PersonUpdateRequest updateRequest = people
                .First()
                .ToPersonUpdateRequest();

            Assert.NotEqual(updateRequest.Name, updatedName);
            Assert.NotEqual(updateRequest.Email, updatedEmail);

            updateRequest.Name = updatedName;
            updateRequest.Email = updatedEmail;

            await _peopleService.UpdatePerson(updateRequest);

            PersonResponse updatedPerson = (await _peopleService.GetPersonById(updateRequest.PersonId))!;

            Assert.Equal(updatedPerson.Name, updatedName);
            Assert.Equal(updatedPerson.Email, updatedEmail);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_Null_Id()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _peopleService.DeletePerson(null);
            });
        }

        [Fact]
        public async Task DeletePerson_InvalidId() 
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(addRequest);
            }

            bool deleted = await _peopleService.DeletePerson(Guid.NewGuid());
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();
            int peopleCount = people.Count();

            Assert.False(deleted);
            Assert.Equal(_validPersonAddRequests.Length, peopleCount);
        }

        [Fact]
        public async Task DeletePerson_ValidId()
        {
            PersonResponse person = await _peopleService.AddPerson(_validPersonAddRequests[0]);
            bool deleted = await _peopleService.DeletePerson(person.PersonId);
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            Assert.True(deleted);
            Assert.Empty(people);
        }

        #endregion

        #region Helpers

        private async Task PrepareTestData()
        {
            CountryAddRequest countryRequest1 = new CountryAddRequest()
            {
                CountryName = "Germany"
            };
            CountryAddRequest countryRequest2 = new CountryAddRequest()
            {
                CountryName = "England"
            };
            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

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
