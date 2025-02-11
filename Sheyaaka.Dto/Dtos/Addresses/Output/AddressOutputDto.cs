namespace Sheyaaka.Dto.Dtos.Addresses.Output
{
    public record AddressOutputDto
    {
        public int AddressID { get; set; }  
        public int StoreID { get; set; }
        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
