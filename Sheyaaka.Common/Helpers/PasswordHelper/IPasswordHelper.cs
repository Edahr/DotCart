using Microsoft.AspNetCore.Identity;
using Sheyaaka.Data.Models;

namespace Sheyaaka.Common.Helpers.PasswordHelper
{
    public interface IPasswordHelper
    {
        string HashPassword(User user, string password);
        PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
        List<string> ValidatePassword(string password);
    }
}
