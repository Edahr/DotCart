using DotCart.BLL.Interfaces;
using DotCart.Common.Helpers.PasswordHelper;
using DotCart.DAL.Interfaces;
using DotCart.Data.Models;
using DotCart.Dto.Dtos.Users.Input;
using DotCart.Infrastructure.EmailService;
using Microsoft.AspNetCore.Identity;

namespace DotCart.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IEmailService _emailService;
        public UserService(
            IUserRepository userRepository,
            IPasswordHelper passwordHelper,
            IEmailService emailService
            )
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _emailService = emailService;
        }

        #region Authentication and authorization methods
        public async Task<User?> RegisterUserAsync(RegisterUserInputDto registerUserInputDto)
        {
            await ValidateRegistrationInput(registerUserInputDto);

            //create a new user object
            User newUser = new User();
            newUser.Email = registerUserInputDto.Email;
            newUser.PasswordHash = _passwordHelper.HashPassword(newUser, registerUserInputDto.Password);
            newUser.IsEmailConfirmed = false;
            newUser.Token = Guid.NewGuid().ToString();

            await _userRepository.SaveAsync(newUser);

            //send confirmation email
            await SendConfirmationEmailAsync(newUser);

            return newUser;
        }
        public async Task<User?> AuthenticateUserAsync(LoginInputDto loginInputDto)
        {
            ArgumentNullException.ThrowIfNull(loginInputDto, "login object is null");
            ArgumentException.ThrowIfNullOrWhiteSpace(loginInputDto.Email, "Email is required");
            ArgumentException.ThrowIfNullOrWhiteSpace(loginInputDto.Password, "Password is required");

            var user = await _userRepository.GetByEmailAsync(loginInputDto.Email);

            //User not found, or the email hasn't been confirmed
            if (user == null || !user.IsEmailConfirmed) return null;

            //Making sure the password matches the hashed password
            var result = _passwordHelper.VerifyHashedPassword(user, user.PasswordHash!, loginInputDto.Password);

            return result == PasswordVerificationResult.Success ? user : null;
        }
        public async Task<User?> ConfirmEmailAsync(string email, string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email, "Email is required");
            ArgumentException.ThrowIfNullOrWhiteSpace(token, "Token is required");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;
            if (user.Token != token) return null;

            user.Token = string.Empty;
            user.IsEmailConfirmed = true;
            return await _userRepository.SaveAsync(user);
        }
        #endregion

        #region Recovery Methods
        public async Task<bool?> ResetPasswordRequestAsync(string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email, "Email is required");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            //Saving the guid for the user to reset the password
            user.Token = Guid.NewGuid().ToString();
            await _userRepository.SaveAsync(user);

            await SendResetPasswordEmailAsync(user);

            return true;
        }
        public async Task<User?> ResetPasswordAsync(string email, string token, string newPassword)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email, "Email is required");
            ArgumentException.ThrowIfNullOrWhiteSpace(token, "Token is required");
            ArgumentException.ThrowIfNullOrWhiteSpace(newPassword, "New password is required");
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;
            if (user.Token != token) return null;

            // Validate new password
            if (!IsNewPasswordValid(newPassword))
                return null;

            // Hash the new password
            user.PasswordHash = _passwordHelper.HashPassword(user, newPassword);
            // Clear the token
            user.Token = string.Empty;

            // Save the updated user
            return await _userRepository.SaveAsync(user);
        }
        #endregion

        #region Authorized User Methods
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            // Verify the current password
            if (!IsCurrentPasswordValid(user, currentPassword))
                return false;

            // Validate new password
            if (!IsNewPasswordValid(newPassword))
                return false;

            // Hash the new password
            user.PasswordHash = _passwordHelper.HashPassword(user, newPassword);

            // Save the updated user
            await _userRepository.SaveAsync(user);

            return true;
        }
        public async Task<User?> UpdateUserProfileAsync(string email, UpdateUserProfileInputDto updateUserProfileInputDto)
        {

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            // Update the user's profile fields
            user.FirstName = updateUserProfileInputDto.FirstName ?? user.FirstName;
            user.LastName = updateUserProfileInputDto.LastName ?? user.LastName;
            user.ProfilePictureURL = updateUserProfileInputDto.ProfilePictureURL ?? user.ProfilePictureURL;

            // Save the updated user
            await _userRepository.SaveAsync(user);

            return user;
        }
        #endregion

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        #region Private methods
        private async Task ValidateRegistrationInput(RegisterUserInputDto user)
        {
            ArgumentNullException.ThrowIfNull(user, "Registration object is null");
            ArgumentException.ThrowIfNullOrWhiteSpace(user.Email, "Email is required");
            ArgumentException.ThrowIfNullOrWhiteSpace(user.Password, "Password is required");

            if (await _userRepository.IsEmailExistsAsync(user.Email))
                throw new Exception("Email already exists.");

            var validationErrors = _passwordHelper.ValidatePassword(user.Password);
            if (validationErrors.Any())
                throw new Exception(string.Join(" ", validationErrors));
        }
        private bool IsCurrentPasswordValid(User user, string currentPassword)
        {
            var passwordVerificationResult = _passwordHelper.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
            return passwordVerificationResult != PasswordVerificationResult.Failed;
        }
        private bool IsNewPasswordValid(string newPassword)
        {
            return !_passwordHelper.ValidatePassword(newPassword).Any();
        }
        private async Task SendConfirmationEmailAsync(User user)
        {
            var confirmLink = $"https://localhost:44394/api/users/confirm-email?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(user.Token!)}";
            var emailBody = $"<h1>Confirm Your Email</h1><p>Click <a href='{confirmLink}'>here</a> to verify your email.</p>";

            await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", emailBody);
        }
        private async Task SendResetPasswordEmailAsync(User user)
        {
            var emailBody = $"Resetting Password Key : {user.Token}";

            await _emailService.SendEmailAsync(user.Email!, "Reset Your password", emailBody);
        }
        #endregion
    }
}
