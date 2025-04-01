using DotCart.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace DotCart.Common.Helpers.PasswordHelper
{
    public interface IPasswordHelper
    {
        string HashPassword(User user, string password);
        PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
        List<string> ValidatePassword(string password);
    }
}
