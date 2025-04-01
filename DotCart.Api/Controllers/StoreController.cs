using DotCart.BLL.Interfaces;
using DotCart.Common.Constants;
using DotCart.Dto.Dtos.Products.Output;
using DotCart.Dto.Dtos.Stores.Input;
using DotCart.Dto.Dtos.Stores.Output;
using DotCart.Dto.MappingProfiles;
using DotCart.Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DotCart.API.Controllers
{
    [Route("api/stores")]
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        public StoreController(IStoreService storeService, IProductService productService, ICacheManager cacheManager)
        {
            _storeService = storeService;
            _productService = productService;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Creates a new store.
        /// </summary>
        /// <param name="createStoreInputDto">The store information to create.</param>
        /// <returns>Returns the created store details.</returns>
        /// <remarks>
        /// This endpoint allows users to create a new store. The user must be authenticated.
        /// </remarks>
        /// <response code="201">Returns the created store.</response>
        /// <response code="400">If store creation fails.</response>
        [HttpPost]
        [ProducesResponseType(typeof(StoreOutputDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CreateStore([FromBody, Required] CreateStoreInputDto createStoreInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var newStore = await _storeService.CreateStoreAsync(createStoreInputDto, userId);

                if (newStore == null)
                {
                    return BadRequest(new { error = "Store Creation Failed" });
                }

                return CreatedAtAction(nameof(GetStoreById), new { id = newStore.UserID }, newStore.ToStoreOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing store.
        /// </summary>
        /// <param name="id">The unique identifier of the store to update.</param>
        /// <param name="updateStoreInputDto">The updated store information.</param>
        /// <returns>Returns the updated store details.</returns>
        /// <remarks>
        /// This endpoint allows updating an existing store's details by its ID.
        /// </remarks>
        /// <response code="200">Returns the updated store.</response>
        /// <response code="404">If the store is not found.</response>
        /// <response code="400">If the update fails.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StoreOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateStore(int id, [FromBody, Required] UpdateStoreInputDto updateStoreInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var updatedStore = await _storeService.UpdateStoreAsync(updateStoreInputDto, id, userId);

                if (updatedStore == null)
                {
                    return NotFound(new { error = "Store was not found" });
                }

                return Ok(updatedStore.ToStoreOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an existing store.
        /// </summary>
        /// <param name="id">The unique identifier of the store to delete.</param>
        /// <returns>Returns the deleted store details.</returns>
        /// <remarks>
        /// This endpoint allows deleting a store by its ID.
        /// </remarks>
        /// <response code="200">Returns the deleted store.</response>
        /// <response code="404">If the store is not found.</response>
        /// <response code="400">If the deletion fails.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(StoreOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var deletedStore = await _storeService.DeleteStoreAsync(id, userId);

                if (deletedStore == null)
                {
                    return NotFound(new { error = "Store was not found" });
                }

                //invalidating cache
                InvalidateCache(id);

                return Ok(deletedStore.ToStoreOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a list of stores associated with the authenticated user.
        /// </summary>
        /// <returns>Returns a list of stores owned by the user.</returns>
        /// <remarks>
        /// This endpoint fetches stores associated with the current authenticated user.
        /// </remarks>
        /// <response code="200">Returns a list of the user's stores.</response>
        /// <response code="404">If no stores are found.</response>
        /// <response code="400">If fetching stores fails.</response>
        [HttpGet("mylist")]
        [ProducesResponseType(typeof(IEnumerable<StoreOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetMyStores()
        {
            var userId = GetUserIdFromClaims();

            var stores = await _storeService.GetUserStoresAsync(userId);
            if (stores == null) return NotFound();

            return Ok(stores.Select(s => s.ToStoreOutputDto()));
        }

        /// <summary>
        /// Retrieves all products of a store, filtered by deletion status.
        /// </summary>
        /// <param name="storeId">The unique identifier of the store.</param>
        /// <param name="isDeleted">The deletion status of the products (true/false).</param>
        /// <returns>Returns a list of products for the store based on their deletion status.</returns>
        /// <remarks>
        /// This endpoint can fetch all active or deleted products for a store based on the provided status.
        /// </remarks>
        /// <response code="200">Returns a list of products for the store.</response>
        /// <response code="404">If no products are found for the store.</response>
        /// <response code="400">If fetching products fails.</response>
        [HttpGet("{storeId}/products/{isDeleted}")]
        [ProducesResponseType(typeof(IEnumerable<ProductOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetStoreProductsByStatus(int storeId, bool isDeleted)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var storeProducts = await _productService.GetProductsByStoreIdAndStatusAsync(storeId, isDeleted, userId);

                return Ok(storeProducts.Select(sp => sp.ToProductOutputDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #region Allow Anonymous
        /// <summary>
        /// Retrieves a list of all stores.
        /// </summary>
        /// <returns>Returns a list of stores.</returns>
        /// <remarks>
        /// This endpoint allows public access to view all stores.
        /// </remarks>
        /// <response code="200">Returns a list of stores.</response>
        /// <response code="400">If fetching stores fails.</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StoreOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            if (stores == null) return NotFound();

            return Ok(stores.Select(s => s.ToStoreOutputDto()));
        }

        /// <summary>
        /// Retrieves a store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the store.</param>
        /// <returns>Returns the store details.</returns>
        /// <remarks>
        /// This endpoint allows public access to view a specific store.
        /// </remarks>
        /// <response code="200">Returns the details of the store.</response>
        /// <response code="404">If the store is not found.</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoreOutputDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _storeService.GetStoreAsync(id);
            if (store == null) return NotFound();

            return Ok(store.ToStoreOutputDto());
        }

        /// <summary>
        /// Retrieves the products of a store (for non-store owners to browse products).
        /// </summary>
        /// <param name="storeId">The unique identifier of the store.</param>
        /// <returns>Returns a list of products available in the store.</returns>
        /// <remarks>
        /// This endpoint allows public access to browse products in a store.
        /// </remarks>
        /// <response code="200">Returns a list of products in the store.</response>
        /// <response code="404">If the store or its products are not found.</response>
        /// <response code="400">If fetching products fails.</response>
        [AllowAnonymous]
        [HttpGet("{storeId}/products")]
        [ProducesResponseType(typeof(IEnumerable<ProductOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetStoreProducts(int storeId)
        {
            try
            {
                if (!_cacheManager.TryGet($"{CacheKeys.StoreProducts}_{storeId}", out IEnumerable<ProductOutputDto> cachedProducts))
                {
                    var storeProducts = await _productService.GetProductsByStoreIdAndStatusAsync(storeId);

                    cachedProducts = storeProducts.Select(sp => sp.ToProductOutputDto());

                    // Set the data in cache with a 10-minute expiration policy
                    _cacheManager.Set($"{CacheKeys.StoreProducts}_{storeId}", cachedProducts, TimeSpan.FromMinutes(10));
                }
                return Ok(cachedProducts);

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        #endregion

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
