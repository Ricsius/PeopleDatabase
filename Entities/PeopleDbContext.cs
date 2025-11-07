using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Entities
{
    public class PeopleDbContext : DbContext
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> People { get; set; }

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
            Person[] people = JsonSerializer.Deserialize<Person[]>(peopleJson)!;

            foreach (Person person in people)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            modelBuilder.Entity<Person>().Property(p => p.Tin)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");
            /*
            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Tin).IsUnique();
            */

            modelBuilder.Entity<Person>()
                .ToTable(t => t.HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8"));

            /*
            modelBuilder.Entity<Person>(e =>
            {
                e.HasOne<Country>(p => p.Country)
                .WithMany(c => c.People)
                .HasForeignKey(c => c.CountryId);
            });
            */
        }

        public virtual Person[] Sp_GetAllPeople() 
        {
            IQueryable<Person> result = People.FromSqlRaw("EXECUTE [dbo].[GetAllPeople]");

            return result.ToArray();
        }

        public virtual int Sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", person.Id),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            };

            string command = "EXECUTE [dbo].[InsertPerson] @Id, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters";

            int result = Database.ExecuteSqlRaw(command, parameters);

            return result;
        }
    }
}
