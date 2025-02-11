using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Stores.Output;

namespace Sheyaaka.Dto.MappingProfiles
{
    public static class StoreMappingProfile
    {
        //store entity to store output dto
        public static StoreOutputDto ToStoreOutputDto(this Store store)
        {
            return new StoreOutputDto
            {
                StoreID = store.StoreID,
                UserID = store.UserID,
                StoreName = store.StoreName,
                IsActive = store.IsActive,
                LogoURL = store.LogoURL,
            };
        }
    }
}
