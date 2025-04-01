using DotCart.Data.Models;

namespace DotCart.Common.Helpers.JWTHelper
{
    public interface IJwtHelper
    {
        string GenerateToken(User user);
    }
}
