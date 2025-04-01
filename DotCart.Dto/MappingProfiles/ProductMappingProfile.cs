using DotCart.Data.Models;
using DotCart.Dto.Dtos.Products.Output;

namespace DotCart.Dto.MappingProfiles
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
