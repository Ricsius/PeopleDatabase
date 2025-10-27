using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> People { get; set; }

        public PeopleDbContext(DbContextOptions options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable(nameof(Countries));
            modelBuilder.Entity<Person>().ToTable(nameof(People));

            string countriesJson = File.ReadAllText("countries.json");
            Country[] countries = JsonSerializer.Deserialize<Country[]>(countriesJson)!;

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            string peopleJson = File.ReadAllText("people.json");
            Person[] people = JsonSerializer.Deserialize<Person[]>(countriesJson)!;

            foreach (Person person in people)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
        }
    }
}
