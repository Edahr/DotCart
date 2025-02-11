namespace Sheyaaka.Dto.Dtos.Brands.Output
{
    public record BrandOutputDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }
}
