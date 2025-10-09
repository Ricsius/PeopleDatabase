using Entities;
using System.Text;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of PeopleService methods
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Age { get; set; }
        public string? Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Id: {PersonId}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Date of Birth: {DateOfBirth.ToString("yyyy-MM-dd")}");
            sb.AppendLine($"Age: {Age}");
            sb.AppendLine($"Gender: {Gender}");
            sb.AppendLine($"CountryId: {CountryId}");
            sb.AppendLine($"CountryName: {CountryName}");
            sb.AppendLine($"Address: {Address}");
            sb.AppendLine($"Receive News Letters: {ReceiveNewsLetters}");

            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }

            PersonResponse? other = obj as PersonResponse;

            return PersonId.Equals(other!.PersonId)
                && Name!.Equals(other!.Name)
                && Email!.Equals(other!.Email)
                && DateOfBirth.Equals(other!.DateOfBirth)
                && Age.Equals(other!.Age)
                && Gender!.Equals(other!.Gender)
                && CountryId.Equals(other!.CountryId)
                && Address!.Equals(other!.Address)
                && ReceiveNewsLetters.Equals(other!.ReceiveNewsLetters);
        }

        public override int GetHashCode()
        {
            return PersonId.GetHashCode()
                & Name!.GetHashCode()
                & Email!.GetHashCode()
                & DateOfBirth.GetHashCode()
                & Age.GetHashCode()
                & Gender!.GetHashCode()
                & CountryId.GetHashCode()
                & Address!.GetHashCode()
                & ReceiveNewsLetters.GetHashCode();
        }
    }

    public static class PersonResponseExtensions 
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            double age = (DateTime.Now - person.DateOfBirth).TotalDays / 365.25;
            age = Math.Floor(age);
            PersonResponse response = new PersonResponse()
            {
                PersonId = person.Id,
                Name = person.Name,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Age = age,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
            };

            return response;
        }
    }
}
