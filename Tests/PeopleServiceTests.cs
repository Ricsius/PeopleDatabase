using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests
{
    public class PeopleServiceTests
    {
        private readonly IPeopleService _peopleService;
        private PersonAddRequest[] _validPersonAddRequests;
        private readonly IFixture _fixture;
        private readonly ITestOutputHelper _outputHelper;
        private readonly List<Person> _people = new List<Person>();
        private readonly Mock<ILogger<PeopleService>> loggerMock = new Mock<ILogger<PeopleService>>();

        public PeopleServiceTests(ITestOutputHelper outputHelper)
        {
            IPeopleRepository mockRepository = TestHelper.CreateMockPeopleRepository(_people);

            _peopleService = new PeopleService(mockRepository, loggerMock.Object);
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
            Func<Task> action = async () =>
            {
                await _peopleService.AddPerson(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_NullName()
        {
            PersonAddRequest request = _fixture
                .Build <PersonAddRequest>()
                .With(p => p.Name, null as string)
                .Create();

            Func<Task> action = async () =>
            {
                await _peopleService.AddPerson(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            PersonAddRequest request = _validPersonAddRequests.First();
            PersonResponse response = await _peopleService.AddPerson(request);
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            response.PersonId.Should().NotBe(Guid.Empty);
            people.Should().Contain(response);
        }

        #endregion

        #region GetPersonById

        [Fact]
        public async Task GetPersonById_NullId() 
        {
            Func<Task> action = async () =>
            {
                await _peopleService.GetPersonById(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetPersonById_ValidId() 
        {
            PersonAddRequest personRequest = _validPersonAddRequests.First();
            PersonResponse? responseFromAdd = await _peopleService.AddPerson(personRequest);
            PersonResponse? responseFromGet = await _peopleService.GetPersonById(responseFromAdd.PersonId);

            responseFromGet.Should().Be(responseFromAdd);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_Empty() 
        {
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons(); 

            people.Should().BeEmpty();
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

            people.Should().BeEquivalentTo(peopleFromAdd);
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

            people.Should().BeEquivalentTo(peopleFromAdd);
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

            expectedPeople.Any().Should().BeTrue();

            IEnumerable<PersonResponse> actualPeople = await _peopleService.SearchPeople((nameof(PersonResponse.Name)), searchText);

            PrintActualElements(actualPeople);

            actualPeople.Should().OnlyContain(p => p.Name!.Contains(searchText, StringComparison.OrdinalIgnoreCase));
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

            peopleArray.Should().BeInDescendingOrder(p => p.Name);
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_Null_Request()
        {
            Func<Task> action = async () =>
            {
                await _peopleService.UpdatePerson(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidId()
        {
            PersonUpdateRequest request = _fixture
                .Build<PersonUpdateRequest>()
                .With(p => p.PersonId, Guid.NewGuid())
                .Create();

            Func<Task> action = async () =>
            {
                await _peopleService.UpdatePerson(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_Null_PersonName()
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = await _peopleService.AddPerson(addRequest);
            }

            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();
            PersonUpdateRequest updateRequest = _fixture
                .Build<PersonUpdateRequest>()
                .With(p => p.Name, null as string)
                .Create();

            Func<Task> action = async () =>
            {
                await _peopleService.UpdatePerson(updateRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
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

            updatedName.Should().NotBe(updateRequest.Name);
            updatedEmail.Should().NotBe(updateRequest.Email);

            updateRequest.Name = updatedName;
            updateRequest.Email = updatedEmail;

            await _peopleService.UpdatePerson(updateRequest);

            PersonResponse updatedPerson = (await _peopleService.GetPersonById(updateRequest.PersonId))!;

            updatedPerson.Name.Should().Be(updatedName);
            updatedPerson.Email.Should().Be(updatedEmail);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_Null_Id()
        {
            Func<Task> action = async () =>
            {
                await _peopleService.DeletePerson(null);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
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

            deleted.Should().BeFalse();
            peopleCount.Should().Be(_validPersonAddRequests.Length);
        }

        [Fact]
        public async Task DeletePerson_ValidId()
        {
            PersonResponse person = await _peopleService.AddPerson(_validPersonAddRequests.First());
            bool deleted = await _peopleService.DeletePerson(person.PersonId);
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();

            deleted.Should().BeTrue();
            people.Should().BeEmpty();
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
