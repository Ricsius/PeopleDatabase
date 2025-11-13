using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
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
        private PersonAddRequest[] _validPersonAddRequests;
        private readonly IFixture _fixture;
        private readonly ITestOutputHelper _outputHelper;
        private readonly List<Country> _countries = new List<Country>();
        private readonly List<Person> _people = new List<Person>();

        public PeopleServiceTests(ITestOutputHelper outputHelper)
        {
            DbContextOptions options = new DbContextOptionsBuilder<PeopleDbContext>().Options;
            DbContextMock<PeopleDbContext> mockContext = new DbContextMock<PeopleDbContext>(options);

            mockContext.CreateDbSetMock(c => c.Countries, _countries);
            mockContext.CreateDbSetMock(c => c.People, _people);

            _peopleService = new PeopleService(mockContext.Object);
            _outputHelper = outputHelper;
            _fixture = new Fixture();

            int n = 0;
            Func<string> nameFactory = () => $"joel_{n++}";

            _validPersonAddRequests = _fixture
                .Build<PersonAddRequest>()
                .With(r => r.Name, nameFactory)
                .With(r => r.Email,"someone@example.com")
                .CreateMany()
                .ToArray();
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
            PersonAddRequest request = _fixture
                .Build <PersonAddRequest>()
                .With(p => p.Name, null as string)
                .Create();

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
            PersonAddRequest personRequest = _validPersonAddRequests.First();
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

            IEnumerable<PersonResponse> expectedPeople = peopleFromAdd.Where(p => p.Name!.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            PrintExpectedElements(expectedPeople);

            Assert.True(expectedPeople.Any());

            IEnumerable<PersonResponse> actualPeople = await _peopleService.SearchPeople((nameof(PersonResponse.Name)), searchText);

            PrintActualElements(actualPeople);

            foreach (PersonResponse person in expectedPeople)
            {
                Assert.Contains(person, actualPeople);
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
            PersonUpdateRequest request = _fixture
                .Build<PersonUpdateRequest>()
                .With(p => p.PersonId, Guid.NewGuid())
                .Create();

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
            PersonResponse person = await _peopleService.AddPerson(_validPersonAddRequests.First());
            bool deleted = await _peopleService.DeletePerson(person.PersonId);
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            Assert.True(deleted);
            Assert.Empty(people);
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
