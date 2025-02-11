namespace Sheyaaka.Dto.Dtos.Stores.Output
{
    public record StoreOutputDto
    {
        public int StoreID { get; set; }
        public int UserID { get; set; }
        public string? StoreName { get; set; }
        public string? LogoURL { get; set; }
        public bool IsActive { get; set; }
    }
}
