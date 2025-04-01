using System.ComponentModel.DataAnnotations;

namespace DotCart.Data.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }
        public int UserID { get; set; } // Foreign Key
        public string StoreName { get; set; } = string.Empty;
        public string? LogoURL { get; set; }
        public bool IsActive { get; set; }

        // Relationships
        public User User { get; set; } = null!;  // Required (One User -> Many Stores)
        public ICollection<Address> Addresses { get; set; } = new List<Address>();  // One Store -> Many Addresses
        public ICollection<Product> Products { get; set; } = new List<Product>();  // One Store -> Many Products
        public ICollection<StoreBrand> StoreBrands { get; set; } = new List<StoreBrand>(); // Many-to-Many with Brand
    }
}
