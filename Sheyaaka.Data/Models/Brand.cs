using System.ComponentModel.DataAnnotations;

namespace Sheyaaka.Data.Models
{
    public class Brand
    {
        [Key]
        public int BrandID { get; set; }
        public string BrandName { get; set; } = string.Empty;

        // Many-to-Many Relationship
        public ICollection<StoreBrand> StoreBrands { get; set; } = new List<StoreBrand>();
        public ICollection<Product> Products { get; set; } = new List<Product>();  // One Brand -> Many Products
    }
}
