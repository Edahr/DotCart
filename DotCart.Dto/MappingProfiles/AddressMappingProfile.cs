using DotCart.Data.Models;
using DotCart.Dto.Dtos.Addresses.Output;

namespace DotCart.Dto.MappingProfiles
{
    public static class AddressMappingProfile
    {
        //address entity to address output dto
        public static AddressOutputDto ToAddressOutputDto(this Address address)
        {
            return new AddressOutputDto
            {
                AddressID = address.AddressID,
                AddressLine = address.AddressLine,
                City = address.City,
                IsActive = address.IsActive,
                State = address.State,
                StoreID = address.StoreID,
                ZipCode = address.ZipCode
            };
        }
    }
}
