using System.ComponentModel.DataAnnotations;

namespace Sheyaaka.Data.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }
        public int StoreID { get; set; } // Foreign Key
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Relationship
        public Store Store { get; set; } = null!; // One Store -> Many Addresses
    }
}
