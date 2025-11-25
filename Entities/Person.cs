using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(40)]
        public string? Name { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public Guid CountryId { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public string? Tin { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Id: {Id}, ");
            stringBuilder.Append($"Name: {Name}, ");
            stringBuilder.Append($"Email: {Email}, ");
            stringBuilder.Append($"Date of Birth: {DateOfBirth.ToString("yyyy-MM-dd")}, ");
            stringBuilder.Append($"Gender: {Gender}, ");
            stringBuilder.Append($"Country Id: {CountryId}, ");
            stringBuilder.Append($"Country: {Country?.Name}, ");
            stringBuilder.Append($"Address: {Address}, ");
            stringBuilder.Append($"Receive News Letters: {ReceiveNewsLetters}, ");


            return stringBuilder.ToString();
        }
    }
}
