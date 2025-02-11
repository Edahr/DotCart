using System.ComponentModel.DataAnnotations;

namespace Sheyaaka.Data.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public int StoreID { get; set; } // Foreign Key
        public int BrandID { get; set; } // Foreign Key
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        public bool IsDeleted { get; set; }

        // Relationships
        public Store Store { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
    }
}
