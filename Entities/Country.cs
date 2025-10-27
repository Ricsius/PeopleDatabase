using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(40)]
        public string? Name { get; set; }
    }
}
