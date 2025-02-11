using Sheyaaka.Data.Models;

namespace Sheyaaka.Common.Helpers.JWTHelper
{
    public interface IJwtHelper
    {
        string GenerateToken(User user);
    }
}
