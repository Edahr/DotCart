namespace Sheyaaka.Dto.Dtos.Addresses.Input
{
    public record CreateAddressInputDto
    {
        public int StoreID { get; set; } 
        public required string AddressLine { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
