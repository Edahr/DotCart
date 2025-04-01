using DotCart.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace DotCart.Common.Helpers.PasswordHelper
{
    public class PasswordHelper : IPasswordHelper
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        public List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();
            const string specialCharacters = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/";

            if (string.IsNullOrWhiteSpace(password))
                return new List<string> { "Password is required." };

            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");

            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit.");

            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");

            if (!password.Any(specialCharacters.Contains))
                errors.Add("Password must contain at least one special character.");

            return errors;
        }
    }
}