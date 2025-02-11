namespace Sheyaaka.Dto.Dtos.Products.Input
{
    public record CreateProductInputDto
    {
        public int StoreID { get; set; }
        public int BrandID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        public bool IsDeleted { get; set; }
    }
}
