using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Sheyaaka.BLL.Interfaces;
using Sheyaaka.Common.Constants;
using Sheyaaka.Dto.Dtos.Brands.Input;
using Sheyaaka.Dto.Dtos.Brands.Output;
using Sheyaaka.Dto.MappingProfiles;
using Sheyaaka.Infrastructure.Cache;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sheyaaka.API.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly ICacheManager _cacheManager;

        public BrandController(ICacheManager cacheManager, IBrandService brandService)
        {
            _cacheManager = cacheManager;
            _brandService = brandService;
        }

        /// <summary>
        /// Retrieves a brand by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the brand.</param>
        /// <returns>Returns the brand details if found.</returns>
        /// <remarks>
        /// This endpoint fetches the details of a brand using its unique ID.
        /// </remarks>
        /// <response code="200">Returns the requested brand.</response>
        /// <response code="404">If the brand is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrandOutputDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();

            return Ok(brand.ToBrandOutputDto());
        }

        /// <summary>
        /// Retrieves all brands.
        /// </summary>
        /// <returns>A list of all available brands.</returns>
        /// <remarks>
        /// This endpoint returns all available brands in the system.
        /// </remarks>
        /// <response code="200">Returns a list of brands.</response>
        /// <response code="404">If no brands are found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetBrands()
        {

            // Check if the data is available in the cache
            if (!_cacheManager.TryGet(CacheKeys.Brands, out IEnumerable<BrandOutputDto> brands))
            {
                // If not, fetch from the service
                var brandEntities = await _brandService.GetAllBrandsAsync();

                // Map the entities to DTOs
                brands = brandEntities.Select(a => a.ToBrandOutputDto()).ToList();

                // Set the data in cache with a 10-minute expiration policy
                _cacheManager.Set(CacheKeys.Brands, brands, TimeSpan.FromMinutes(10));
            }

            return Ok(brands);
        }

        /// <summary>
        /// Creates a new brand.
        /// </summary>
        /// <param name="createBrandInputDto">The details of the brand to create.</param>
        /// <returns>Returns the newly created brand.</returns>
        /// <remarks>
        /// This endpoint allows users to create a new brand by providing the necessary details.
        /// </remarks>
        /// <response code="201">Returns the created brand.</response>
        /// <response code="400">If brand creation fails.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BrandOutputDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CreateBrand([FromBody, Required] CreateBrandInputDto createBrandInputDto)
        {
            try
            {
                var newBrand = await _brandService.CreateBrandAsync(createBrandInputDto);

                if (newBrand == null)
                {
                    return BadRequest(new { error = "Brand Creation Failed" });
                }

                //invalidatingCache
                InvalidateCache();

                return Created(nameof(newBrand), newBrand.ToBrandOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing brand.
        /// </summary>
        /// <param name="id">The unique identifier of the brand to update.</param>
        /// <param name="updateBrandInputDto">The updated brand information.</param>
        /// <returns>Returns the updated brand details.</returns>
        /// <remarks>
        /// This endpoint allows updating a brand's details.
        /// </remarks>
        /// <response code="200">Returns the updated brand.</response>
        /// <response code="404">If the brand is not found.</response>
        /// <response code="400">If the update request fails.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BrandOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody, Required] UpdateBrandInputDto updateBrandInputDto)
        {
            try
            {
                var updatedBrand = await _brandService.UpdateBrandAsync(updateBrandInputDto, id);

                if (updatedBrand == null)
                {
                    return NotFound(new { error = "Brand was not found" });
                }

                //invalidatingCache
                InvalidateCache();

                return Ok(updatedBrand.ToBrandOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an existing brand.
        /// </summary>
        /// <param name="id">The unique identifier of the brand to delete.</param>
        /// <returns>Returns the deleted brand details.</returns>
        /// <remarks>
        /// This endpoint allows deleting a brand by its ID.
        /// </remarks>
        /// <response code="200">Returns the deleted brand.</response>
        /// <response code="404">If the brand is not found.</response>
        /// <response code="400">If the deletion fails.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BrandOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var deletedBrand = await _brandService.DeleteBrandAsync(id);

                if (deletedBrand == null)
                {
                    return NotFound(new { error = "Brand was not found" });
                }

                //invalidatingCache
                InvalidateCache();

                return Ok(deletedBrand.ToBrandOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Unassigns a brand from a store.
        /// </summary>
        /// <param name="brandStoreAssignemntInputDto">The brand and store details to unassign.</param>
        /// <returns>Returns the updated store-brand relationship.</returns>
        /// <remarks>
        /// This endpoint requires authorization and removes the association between a brand and a store.
        /// </remarks>
        /// <response code="200">Returns the updated store-brand relationship.</response>
        /// <response code="404">If the brand-store relationship is not found.</response>
        /// <response code="400">If the request fails.</response>
        [Authorize]
        [HttpDelete("store/assignment")]
        [ProducesResponseType(typeof(StoreBrandOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UnassignBrandFromStore([FromBody] BrandStoreAssignmentInputDto brandStoreAssignemntInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var result = await _brandService.UnassignBrandFromStore(
                    brandStoreAssignemntInputDto.BrandId,
                    brandStoreAssignemntInputDto.StoreId,
                    userId
                );

                if (result == null)
                    return NotFound(@$"Could not find a valid Brand-Store relationship between BrandId:{brandStoreAssignemntInputDto.BrandId} And StoreId:{brandStoreAssignemntInputDto.StoreId}");

                return Ok(result.ToStoreBrandOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Assigns a brand to a store.
        /// </summary>
        /// <param name="brandStoreAssignemntInputDto">The brand and store details to assign.</param>
        /// <returns>Returns the created store-brand relationship.</returns>
        /// <remarks>
        /// This endpoint requires authorization and associates a brand with a store.
        /// </remarks>
        /// <response code="200">Returns the created store-brand relationship.</response>
        /// <response code="404">If the brand or store is not found.</response>
        /// <response code="400">If the assignment fails.</response>
        [Authorize]
        [HttpPost("store/assignment")]
        [ProducesResponseType(typeof(StoreBrandOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> AssignBrandToStore([FromBody] BrandStoreAssignmentInputDto brandStoreAssignemntInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var result = await _brandService.AssignBrandToStore(
                    brandStoreAssignemntInputDto.BrandId,
                    brandStoreAssignemntInputDto.StoreId,
                    userId
                );

                if (result == null)
                    return NotFound(@$"Could not create a valid Brand-Store relationship between BrandId:{brandStoreAssignemntInputDto.BrandId} And StoreId:{brandStoreAssignemntInputDto.StoreId}");

                return Ok(result.ToStoreBrandOutputDto());
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

        private void InvalidateCache()
        {
            // Invalidate the cache for brands
            _cacheManager.Remove(CacheKeys.Brands);
        }
    }
}
