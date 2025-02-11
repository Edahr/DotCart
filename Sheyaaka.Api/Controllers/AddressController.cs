using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sheyaaka.BLL.Interfaces;
using Sheyaaka.BLL.Services;
using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Addresses.Input;
using Sheyaaka.Dto.Dtos.Addresses.Output;
using Sheyaaka.Dto.Dtos.Stores.Input;
using Sheyaaka.Dto.Dtos.Stores.Output;
using Sheyaaka.Dto.MappingProfiles;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sheyaaka.API.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Creates a new address.
        /// </summary>
        /// <param name="createAddressInputDto">The address information to create.</param>
        /// <returns>Returns the created address details.</returns>
        /// <remarks>
        /// This endpoint allows users to create a new address. The user must be authenticated.
        /// </remarks>
        /// <response code="201">Returns the created address.</response>
        /// <response code="400">If address creation fails.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AddressOutputDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CreateAddress([FromBody, Required] CreateAddressInputDto createAddressInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var newAddress = await _addressService.CreateAddressAsync(createAddressInputDto, userId);

                if (newAddress == null)
                {
                    return BadRequest(new { error = "Address Creation Failed" });
                }

                return Created(nameof(newAddress), newAddress.ToAddressOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing address.
        /// </summary>
        /// <param name="id">The unique identifier of the address to update.</param>
        /// <param name="updateAddressInputDto">The updated address information.</param>
        /// <returns>Returns the updated address details.</returns>
        /// <remarks>
        /// This endpoint allows updating an address by its ID.
        /// </remarks>
        /// <response code="200">Returns the updated address.</response>
        /// <response code="404">If the address is not found.</response>
        /// <response code="400">If the update fails.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AddressOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody, Required] UpdateAddressInputDto updateAddressInputDto)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var updatedAddress = await _addressService.UpdateAddressAsync(updateAddressInputDto, id, userId);

                if (updatedAddress == null)
                {
                    return NotFound(new { error = "Address was not found" });
                }

                return Ok(updatedAddress.ToAddressOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an existing address.
        /// </summary>
        /// <param name="id">The unique identifier of the address to delete.</param>
        /// <returns>Returns the deleted address details.</returns>
        /// <remarks>
        /// This endpoint allows deleting an address by its ID.
        /// </remarks>
        /// <response code="200">Returns the deleted address.</response>
        /// <response code="404">If the address is not found.</response>
        /// <response code="400">If the deletion fails.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(AddressOutputDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var deletedAddress = await _addressService.DeleteAddressAsync(id, userId);

                if (deletedAddress == null)
                {
                    return NotFound(new { error = "Address was not found" });
                }

                return Ok(deletedAddress.ToAddressOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the list of addresses for a store.
        /// </summary>
        /// <param name="storeId">The unique identifier of the store to fetch addresses for.</param>
        /// <returns>Returns a list of addresses for the store.</returns>
        /// <remarks>
        /// This endpoint allows fetching the addresses associated with a store. 
        /// The user must be authenticated.
        /// </remarks>
        /// <response code="200">Returns a list of addresses for the store.</response>
        /// <response code="404">If no addresses are found for the store.</response>
        [HttpGet("{storeId}")]
        [ProducesResponseType(typeof(IEnumerable<AddressOutputDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetStoreAddresses(int storeId)
        {
            var userId = GetUserIdFromClaims();

            var addresses = await _addressService.GetStoreAddresses(storeId, userId);
            if (addresses == null || !addresses.Any()) return NotFound();

            return Ok(addresses.Select(a => a.ToAddressOutputDto()));
        }

        private int GetUserIdFromClaims()
        {
            var userClaims = User.Claims;
            var Id = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(Id, out int parsedId))
                throw new BadHttpRequestException("Failed to parse UserId, Check your JWT");

            return parsedId;
        }
    }
}
