using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Sheyaaka.Configuration.Middlewares
{
    public class JwtIPBindingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtIPBindingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //Getting the Token from the second part of the Authorization header
                var token = GetTokenFromHeader(context);

                //This logic would only apply to endpoints that require authentication
                if (!string.IsNullOrWhiteSpace(token))
                {
                    //Getting the IP address of the client
                    var ipAddress = GetClientIpAddress(context);

                    //Getting the IP address from the token
                    var tokenIp = GetTokenIpAddress(token);

                    if (string.IsNullOrWhiteSpace(tokenIp) || tokenIp != ipAddress)
                    {
                        await RejectRequest(context, "Token IP mismatch");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                await RejectRequest(context, "Invalid token");
                return;
            }

            await _next(context);
        }

        private string? GetTokenFromHeader(HttpContext context)
        {
            return context.Request.Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last();
        }

        private string? GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // If the IP address is empty, try to get it from the X-Forwarded-For header
            return string.IsNullOrWhiteSpace(ipAddress)
                ? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()
                : ipAddress;
        }

        private string? GetTokenIpAddress(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "IPAddress")?.Value;
        }

        private async Task RejectRequest(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(message);
        }
    }
}
