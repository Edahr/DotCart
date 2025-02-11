using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sheyaaka.BLL.Interfaces;
using Sheyaaka.Dto.Dtos.Products.Input;
using Sheyaaka.Dto.Dtos.Products.Output;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Sheyaaka.Dto.MappingProfiles;
using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Brands.Output;
using Sheyaaka.Infrastructure.Cache;
using Sheyaaka.Common.Constants;

namespace Sheyaaka.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        public ProductController(IProductService productService, ICacheManager cacheManager)
        {
            _productService = productService;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to create a new product.
        /// </remarks>
        /// <param name="createProductInputDto">The product details.</param>
        /// <returns>The created product details.</returns>
        /// <response code="201">Returns the created product.</response>
        /// <response code="400">If product creation fails.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductOutputDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CreateProduct([FromBody, Required] CreateProductInputDto createProductInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var newProduct = await _productService.CreateProductAsync(createProductInputDto, userId);

                if (newProduct == null)
                {
                    return BadRequest(new { error = "Product Creation Failed" });
                }

                //invalidating the store Products cache
                InvalidateCache(createProductInputDto.StoreID);

                return Created(nameof(newProduct), newProduct.ToProductOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to update an existing product.
        /// </remarks>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="updateProductInputDto">The updated product details.</param>
        /// <returns>The updated product details.</returns>
        /// <response code="200">Returns the updated product.</response>
        /// <response code="400">If the update request is invalid.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody, Required] UpdateProductInputDto updateProductInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(updateProductInputDto, id, userId);

                if (updatedProduct == null)
                {
                    return NotFound(new { error = "Product was not found" });
                }

                //invalidating the store Products cache
                InvalidateCache(updateProductInputDto.StoreID);

                return Ok(updatedProduct.ToProductOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Changes the deletion status of a product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows an authenticated user to delete or recover a product.
        /// </remarks>
        /// <param name="id">The ID of the product.</param>
        /// <param name="isDeleted">Set to `true` to delete the product, or `false` to recover it.</param>
        /// <returns>The updated product details.</returns>
        /// <response code="200">Returns the updated product.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPut("{id}/deletionstatus")]
        [ProducesResponseType(typeof(ProductOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ChangeProductDeletionStatus(int id, bool isDeleted)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var changedProduct = await _productService.ChangeProductDeletionStatusAsync(id, isDeleted, userId);

                if (changedProduct == null)
                {
                    return NotFound(new { error = "Product was not found" });
                }

                //invalidating the store Products cache
                InvalidateCache(changedProduct.StoreID);

                return Ok(changedProduct.ToProductOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private int GetUserIdFromClaims()
        {
            var userClaims = User.Claims;
            var Id = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(Id, out int parsedId))
                throw new BadHttpRequestException("Failed to parse UserId, Check your JWT");

            return parsedId;
        }

        private void InvalidateCache(int storeId)
        {
            // Invalidate the cache for brands
            _cacheManager.Remove($"{CacheKeys.StoreProducts}_{storeId}");
        }
    }
}
