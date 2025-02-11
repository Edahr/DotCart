namespace Sheyaaka.Dto.Dtos.Stores.Input
{
    public record UpdateStoreInputDto
    {
        public required string StoreName { get; set; }
        public string? LogoURL { get; set; }
        public bool IsActive { get; set; }
    }
}
