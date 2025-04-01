namespace DotCart.Dto.Dtos.Products.Output
{
    public record ProductOutputDto
    {
        public int ProductId { get; set; }
        public int StoreID { get; set; }
        public int BrandID { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        public bool IsDeleted { get; set; }
    }
}
