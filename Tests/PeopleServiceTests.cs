using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;

namespace Tests
{
    public class PeopleServiceTests
    {
        private readonly IPeopleService _peopleService;

        public PeopleServiceTests()
        {
            _peopleService = new PeopleService();
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
            PersonAddRequest request = new PersonAddRequest()
            {
                Name = "Joe",
                Email = "dummy@example.com",
                Address = "Sample street",
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2001-01-01"),
                ReceiveNewsLetters = true,
            };

            PersonResponse response = _peopleService.AddPerson(request);

            IEnumerable<PersonResponse> people = _peopleService.GetAllPersons();

            Assert.True(response.PersonId != Guid.Empty);
            Assert.Contains(response, people);
        }

        #endregion
    }
}
