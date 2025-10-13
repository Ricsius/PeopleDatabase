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
            _peopleService = new PeopleService(false);
            _countriesService = new CountriesService(false);
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

            IEnumerable<PersonResponse> people = _peopleService.SearchPeople((nameof(PersonResponse.Name)), "");

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

            PrintExpectedElements(peopleFromAdd.Where(p => p.Name!.Contains(searchText, StringComparison.OrdinalIgnoreCase)));

            IEnumerable<PersonResponse> people = _peopleService.SearchPeople((nameof(PersonResponse.Name)), searchText);

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
        public void GetSortedPeople_SortBy_Name_Descending()
        {
            List<PersonResponse> peopleFromAdd = new List<PersonResponse>();

            foreach (PersonAddRequest request in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(request);

                peopleFromAdd.Add(response);
            }

            PersonResponse[] expectedPeople = peopleFromAdd
                .OrderByDescending(p => p.Name)
                .ToArray();

            PrintExpectedElements(expectedPeople);

            PersonResponse[] people = _peopleService
                .GetSortedPeople(peopleFromAdd, (nameof(Person.Name)), SortOrderOptions.Descending)
                .ToArray();

            PrintActualElements(people);

            for (int i = 0; i < expectedPeople.Length; i++)
            {
                Assert.Equal(expectedPeople[i], people[i]);
            }
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public void UpdatePerson_Null_Request()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                _peopleService.UpdatePerson(null);
            });
        }

        [Fact]
        public void UpdatePerson_InvalidId()
        {
            PersonUpdateRequest request = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid()
            };
            Assert.Throws<ArgumentException>(() =>
            {
                _peopleService.UpdatePerson(request);
            });
        }

        [Fact]
        public void UpdatePerson_Null_PersonName()
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(addRequest);
            }

            PersonUpdateRequest updateRequest = _peopleService
                .GetAllPersons()
                .First()
                .ToPersonUpdateRequest();
            updateRequest.Name = null;

            Assert.Throws<ArgumentException>(() =>
            {
                _peopleService.UpdatePerson(updateRequest);
            });
        }

        [Fact]
        public void UpdatePerson_Update_Name_Email()
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(addRequest);
            }

            string updatedName = "UpdatedName";
            string updatedEmail = "UpdatedEmailAddress@example.com";
            PersonUpdateRequest updateRequest = _peopleService
                .GetAllPersons()
                .First()
                .ToPersonUpdateRequest();

            Assert.NotEqual(updateRequest.Name, updatedName);
            Assert.NotEqual(updateRequest.Email, updatedEmail);

            updateRequest.Name = updatedName;
            updateRequest.Email = updatedEmail;

            _peopleService.UpdatePerson(updateRequest);

            PersonResponse updatedPerson = _peopleService.GetPersonById(updateRequest.PersonId)!;

            Assert.Equal(updatedPerson.Name, updatedName);
            Assert.Equal(updatedPerson.Email, updatedEmail);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public void DeletePerson_Null_Id()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _peopleService.DeletePerson(null);
            });
        }

        [Fact]
        public void DeletePerson_InvalidId() 
        {
            foreach (PersonAddRequest addRequest in _validPersonAddRequests)
            {
                PersonResponse response = _peopleService.AddPerson(addRequest);
            }

            bool deleted = _peopleService.DeletePerson(Guid.NewGuid());
            int peopleCount = _peopleService.GetAllPersons().Count();

            Assert.False(deleted);
            Assert.Equal(_validPersonAddRequests.Length, peopleCount);
        }

        [Fact]
        public void DeletePerson_ValidId()
        {
            PersonResponse person = _peopleService.AddPerson(_validPersonAddRequests[0]);
            bool deleted = _peopleService.DeletePerson(person.PersonId);
            int peopleCount = _peopleService.GetAllPersons().Count();

            Assert.True(deleted);
            Assert.Empty(_peopleService.GetAllPersons());
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
