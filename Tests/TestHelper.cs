using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests
{
    internal static class TestHelper
    {
        public static PeopleDbContext CreateMockPeopleDbContext(ICollection<Country> countryCollection, ICollection<Person> personCollection)
        {
            DbSet<Country> countriesSet = CreateMockCountryDbSet(countryCollection);
            DbSet<Person> peopleSet = CreateMockPersonDbSet(personCollection);
            DbContextOptions options = new DbContextOptionsBuilder<PeopleDbContext>().Options;
            Mock<PeopleDbContext> mockContext = new Mock<PeopleDbContext>(options);

            mockContext.Setup(m => m.Countries).Returns(countriesSet);
            mockContext.Setup(m => m.People).Returns(peopleSet);
            mockContext.Setup(m => m.Sp_GetAllPeople()).Returns(() => personCollection.ToArray());
            mockContext.Setup(m => m.Sp_InsertPerson(It.IsAny<Person>())).Callback<Person>(p => personCollection.Add(p));

            return mockContext.Object;
        }

        private static DbSet<Country> CreateMockCountryDbSet(ICollection<Country> countryCollection)
        {
            IQueryable<Country> queryable = countryCollection.AsQueryable();
            Mock<DbSet<Country>> mockCountriesSet = new Mock<DbSet<Country>>();
            mockCountriesSet.Setup(m => m.Add(It.IsAny<Country>())).Callback<Country>(c => countryCollection.Add(c));
            mockCountriesSet.As<IQueryable<Country>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockCountriesSet.As<IQueryable<Country>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockCountriesSet.As<IQueryable<Country>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockCountriesSet.As<IQueryable<Country>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockCountriesSet.Object;
        }

        private static DbSet<Person> CreateMockPersonDbSet(ICollection<Person> personCollection)
        {
            IQueryable<Person> queryable = personCollection.AsQueryable();
            Mock<DbSet<Person>> mockPeopleSet = new Mock<DbSet<Person>>();
            mockPeopleSet.Setup(m => m.Add(It.IsAny<Person>())).Callback<Person>(c => personCollection.Add(c));
            mockPeopleSet.Setup(m => m.Remove(It.IsAny<Person>())).Callback<Person>(c => personCollection.Remove(c));
            mockPeopleSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockPeopleSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockPeopleSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockPeopleSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockPeopleSet.Object;
        }
    }
}
