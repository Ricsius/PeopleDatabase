using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Name can't be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be in a valid format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Date of Birth can't be blank")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender can't be blank")]
        public GenderOptions Gender { get; set; }

        [Required(ErrorMessage = "CountryId can't be blank")]
        public Guid CountryId { get; set; }

        [Required(ErrorMessage = "Address can't be blank")]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson() 
        {
            return new Person
            {
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
