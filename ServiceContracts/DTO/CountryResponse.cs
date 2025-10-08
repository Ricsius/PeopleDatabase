using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }

            CountryResponse? other = obj as CountryResponse;

            return this.CountryId == other!.CountryId
                && this.CountryName == other!.CountryName;
        }

        public override int GetHashCode()
        {
            return CountryId!.GetHashCode() 
                & CountryName!.GetHashCode();
        }
    }

    public static class CountryExtensions 
    {
        public static CountryResponse ToCountryResponse(this Country country) 
        {
            return new CountryResponse() 
            {
                CountryId = country.Id,
                CountryName = country.Name
            };
        }
    }
}
