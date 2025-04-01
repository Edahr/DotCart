using DotCart.Data.Models;
using DotCart.Dto.Dtos.Brands.Output;

namespace DotCart.Dto.MappingProfiles
{
    public static class BrandMappingProfile
    {
        //Brand entity to Brand output dto
        public static BrandOutputDto ToBrandOutputDto(this Brand brand)
        {
            return new BrandOutputDto
            {
                BrandId = brand.BrandID,
                BrandName = brand.BrandName
            };
        }

        public static StoreBrandOutputDto ToStoreBrandOutputDto(this StoreBrand storeBrand)
        {
            return new StoreBrandOutputDto() { BrandId = storeBrand.BrandID, StoreId = storeBrand.StoreID };
        }
    }
}
