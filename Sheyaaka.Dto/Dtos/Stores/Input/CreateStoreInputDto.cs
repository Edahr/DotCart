namespace Sheyaaka.Dto.Dtos.Stores.Input
{
    public record CreateStoreInputDto
    {
        public required string StoreName { get; set; }
        public string? LogoURL { get; set; }
        public bool IsActive { get; set; }
    }
}
