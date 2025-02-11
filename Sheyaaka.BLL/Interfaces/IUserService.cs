using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Users.Input;
using SheyaakaDto.Dtos.Users.Input;

namespace Sheyaaka.BLL.Interfaces
{
    public interface IUserService
    {
        //Authentication and authorization methods
        Task<User?> RegisterUserAsync(RegisterUserInputDto registerUserInputDto);
        Task<User?> AuthenticateUserAsync(LoginInputDto loginInputDto);
        Task<User?> ConfirmEmailAsync(string email, string token);

        //Authorized User Methods
        Task<bool> ChangePasswordAsync(string userEmail, string currentPassword, string newPassword);
        Task<User?> UpdateUserProfileAsync(string email, UpdateUserProfileInputDto updateUserProfileInputDto);

        //Recovery methods
        Task<bool?> ResetPasswordRequestAsync(string email);
        Task<User?> ResetPasswordAsync(string email, string token, string newPassword);

        //Helper methods
        Task<User?> GetUserByIdAsync(int id);


    }
}
