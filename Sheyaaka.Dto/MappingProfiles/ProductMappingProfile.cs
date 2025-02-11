using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Products.Output;

namespace Sheyaaka.Dto.MappingProfiles
{
    public static class ProductMappingProfile
    {
        //user entity to user output dto
        public static ProductOutputDto ToProductOutputDto(this Product product)
        {
            return new ProductOutputDto
            {
                ProductId = product.ProductID,
                BrandID = product.BrandID,
                StoreID = product.StoreID,
                ImageURL = product.ImageURL,
                IsDeleted = product.IsDeleted,
                Price = product.Price,
                ProductName = product.ProductName,
            };
        }
    }
}
