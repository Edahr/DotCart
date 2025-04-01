using DotCart.BLL.Interfaces;
using DotCart.DAL.Interfaces;
using DotCart.Data.Models;
using DotCart.Dto.Dtos.Addresses.Input;

namespace DotCart.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IStoreRepository _storeRepository;

        public AddressService(IAddressRepository addressRepository, IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
            _addressRepository = addressRepository;
        }

        //Get all store addresses
        public async Task<IEnumerable<Address>> GetStoreAddresses(int storeId, int userId)
        {

            return await _addressRepository.GetStoreAddressesAsync(storeId, userId);
        }
        //Create address async
        public async Task<Address?> CreateAddressAsync(CreateAddressInputDto createAddressInputDto, int userId)
        {
            ArgumentNullException.ThrowIfNull(createAddressInputDto);

            //Checking if the storeId is valid
            if (await CheckIfStoreIsValid(createAddressInputDto.StoreID, userId))
            {
                Address address = new Address();
                address.StoreID = createAddressInputDto.StoreID;
                address.AddressLine = createAddressInputDto.AddressLine;
                address.City = createAddressInputDto.City;
                address.State = createAddressInputDto.State;
                address.ZipCode = createAddressInputDto.ZipCode;
                address.IsActive = createAddressInputDto.IsActive;

                return await _addressRepository.SaveAsync(address) ?? null;
            }
            else
                return null;
        }
        //Update Address async
        public async Task<Address?> UpdateAddressAsync(UpdateAddressInputDto updateAddressAsync, int addressId, int userId)
        {
            ArgumentNullException.ThrowIfNull(updateAddressAsync);

            //Checking if the address exists
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null)
                return null;

            //Checking if the storeId is valid
            if (await CheckIfStoreIsValid(updateAddressAsync.StoreID, userId))
            {
                address.StoreID = updateAddressAsync.StoreID;
                address.AddressLine = updateAddressAsync.AddressLine;
                address.City = updateAddressAsync.City;
                address.State = updateAddressAsync.State;
                address.ZipCode = updateAddressAsync.ZipCode;
                address.IsActive = updateAddressAsync.IsActive;

                return await _addressRepository.SaveAsync(address) ?? null;
            }
            else
                return null;
        }
        //Delete Address async
        public async Task<Address?> DeleteAddressAsync(int addressId, int userId)
        {
            return await _addressRepository.DeleteAddressAsync(addressId, userId);
        }

        //private methods
        private async Task<bool> CheckIfStoreIsValid(int storeId, int userId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            return store != null && store.UserID == userId; //Store exists and belongs to this user
        }
    }
}
