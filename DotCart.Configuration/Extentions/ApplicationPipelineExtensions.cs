using DotCart.Configuration.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace DotCart.Configuration.Extentions
{
    public static class ApplicationPipelineExtensions
    {
        public static void ConfigurePipeline(WebApplication app)
        {
            // If the application is in development mode, enable Swagger for API documentation.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Enables Swagger middleware to serve generated Swagger JSON.
                app.UseSwaggerUI(); // Enables the Swagger UI for easier testing and exploration of APIs.
            }

            // Redirect HTTP requests to HTTPS for secure communication.
            app.UseHttpsRedirection();

            // **Binding JWT to Client IP**
            // Enable Middleware to check if the IP address in the JWT token matches the client's IP address.
            // This is to prevent token theft and misuse.
            app.UseMiddleware<JwtIPBindingMiddleware>();

            // Enable authentication & authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controller routes to handle incoming HTTP requests.
            app.MapControllers();
        }
    }
}
