using Entities;
using Moq;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Tests
{
    internal static class TestHelper
    {
        public static ICountriesRepository CreateMockCountriesRepository(ICollection<Country> countriesCollection)
        {
            Mock<ICountriesRepository> repositoryMock = new Mock<ICountriesRepository>();

            repositoryMock
                .Setup(r => r.GetAllCountries())
                .ReturnsAsync(() => countriesCollection.ToArray());

            repositoryMock
                .Setup(r => r.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => countriesCollection.FirstOrDefault(c => c.Id == id));

            repositoryMock
                .Setup(r => r.GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync((string name) => countriesCollection.FirstOrDefault(c => c.Name == name)); ;

            repositoryMock
                .Setup(r => r.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync((Country c) => 
                {
                    countriesCollection.Add(c);

                    return c;
                });

            return repositoryMock.Object;
        }

        public static IPeopleRepository CreateMockPeopleRepository(ICollection<Person> personCollection)
        {
            Mock<IPeopleRepository> repositoryMock = new Mock<IPeopleRepository>();

            repositoryMock
                .Setup(r => r.GetAllPersons())
                .ReturnsAsync(() => personCollection.ToArray());

            repositoryMock
                .Setup(r => r.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => personCollection.FirstOrDefault(p => p.Id == id));

            repositoryMock
                .Setup(r => r.SearchPeople(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync((Expression<Func<Person, bool>> e) => personCollection.AsQueryable().Where(e));

            repositoryMock
                .Setup(r => r.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync((Person p) => 
            {
                personCollection.Add(p);

                return p;
            });

            repositoryMock
                .Setup(r => r.DeletePersonById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => 
                {
                    Person? foundPerson = personCollection.FirstOrDefault(p => p.Id == id);

                    if (foundPerson != null)
                    {
                        return personCollection.Remove(foundPerson);
                    }

                    return false;
                });

            repositoryMock
                .Setup(r => r.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync((Person p) => p);

            return repositoryMock.Object;
        }
    }
}
