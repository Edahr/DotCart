using DotCart.BLL.Interfaces;
using DotCart.DAL.Interfaces;
using DotCart.Data.Models;
using DotCart.Dto.Dtos.Products.Input;

namespace DotCart.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> CreateProductAsync(CreateProductInputDto createProductInputDto, int userId)
        {
            //validating the input
            await ValidateProductInputDto(createProductInputDto, userId);

            Product product = new Product();
            product.StoreID = createProductInputDto.StoreID;
            product.BrandID = createProductInputDto.BrandID;
            product.ProductName = createProductInputDto.ProductName;
            product.Price = createProductInputDto.Price;
            product.ImageURL = createProductInputDto.ImageURL;
            product.IsDeleted = createProductInputDto.IsDeleted;

            return await _productRepository.SaveAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(UpdateProductInputDto updateProductInputDto, int productId, int userId)
        {
            //validating the input
            await ValidateProductInputDto(updateProductInputDto, userId);

            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null) return null;

            product.StoreID = updateProductInputDto.StoreID;
            product.BrandID = updateProductInputDto.BrandID;
            product.ProductName = updateProductInputDto.ProductName;
            product.Price = updateProductInputDto.Price;
            product.ImageURL = updateProductInputDto.ImageURL;
            product.IsDeleted = updateProductInputDto.IsDeleted;

            return await _productRepository.SaveAsync(product);
        }

        //This will change the product deletion status (Delete products / Recover deleted Products)
        //If the results were null => User does not have access over this product
        public async Task<Product?> ChangeProductDeletionStatusAsync(int productId, bool isDeleted, int userId)
        {
            var product = await _productRepository.ChangeProductDeletionStatusAsync(productId, isDeleted, userId);

            return product ?? null;

        }

        //This Method serves as the filtration hub for the products; it allows both store owners; and non store owners to filter products.
        public async Task<IEnumerable<Product>> GetProductsByStoreIdAndStatusAsync(int? storeId = null, bool? isDeleted = false, int? userId = null)
        {
            return await _productRepository.GetProductsByStoreIdAndStatusAndUserIdAsync(storeId, isDeleted, userId);
        }

        private async Task ValidateProductInputDto(CreateProductInputDto createProductInputDto, int userId)
        {
            ArgumentNullException.ThrowIfNull(createProductInputDto, "Product Input Dto is null.");
            ArgumentException.ThrowIfNullOrWhiteSpace(createProductInputDto.ProductName, "Product name is required.");
            ArgumentNullException.ThrowIfNull(createProductInputDto, "Product Input Dto is null.");

            //This will validate all ids and make sure the product belonds to a valid store, and a valid brand
            if (!await _productRepository.IsValidProductInformationAsync(createProductInputDto.StoreID, createProductInputDto.BrandID, userId))
                throw new InvalidOperationException("The Ids used in this product Creation request are invalid");
        }
    }
}
