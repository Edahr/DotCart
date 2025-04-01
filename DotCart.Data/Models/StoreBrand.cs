namespace DotCart.Data.Models
{
    public class StoreBrand
    {
        public int StoreID { get; set; }  // Foreign Key
        public int BrandID { get; set; }  // Foreign Key

        // Navigation Properties
        public Store Store { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
    }
}
