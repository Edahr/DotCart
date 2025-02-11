using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sheyaaka.BLL.Interfaces;
using Sheyaaka.BLL.Services;
using Sheyaaka.Common.Helpers.JWTHelper;
using Sheyaaka.Common.Helpers.PasswordHelper;
using Sheyaaka.DAL.Interfaces;
using Sheyaaka.DAL.Repositories;
using Sheyaaka.Data;
using Sheyaaka.Infrastructure.Cache;
using Sheyaaka.Infrastructure.EmailService;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace Sheyaaka.Configuration.Extentions
{
    public class ServiceCollectionExtensions
    {
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            // Add controllers
            builder.Services.AddControllers();

            // Add Swagger with JWT Bearer Authentication
            UseSwagger(builder);

            //Adding HttpContextAccessor so i can use IHttpContextAccessor Within the solution
            builder.Services.AddHttpContextAccessor();

            //use Authentcation and authorization
            UseAuthenticationAndAuthorization(builder);

            //Adding SheyaakaDbContext to the DI Container
            RegisterDbContext(builder);

            //Adding Repositories to the DI Container
            RegisterRepositories(builder);

            //Register Services
            RegisterBllServices(builder);

            //Register Helper classes
            RegisterHelperServices(builder);

            //Register MailKit service
            RegisterSmptService(builder);

            //Register Caching service
            RegisterCachingService(builder);
        }

        //Registering swagger
        public static void UseSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Add the "Authorize" button
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}' in the field below."
                });
                // Do NOT apply security globally - this prevents all endpoints from requiring auth
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

        }

        //Use Authentication and Authorization
        private static void UseAuthenticationAndAuthorization(WebApplicationBuilder builder)
        {
            //Adding Authentication With JWT tokens
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Secret")))
                };
            });

            //Adding Authorization
            builder.Services.AddAuthorization();
        }

        //Registering the DB context
        private static void RegisterDbContext(WebApplicationBuilder builder)
        {
            //Adding SheyaakaDbContext to the DI Container with the connection string
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        //Registering the repositories
        private static void RegisterRepositories(WebApplicationBuilder builder)
        {
            //Registering the generic repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //Registering the user repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            //Registering the store repository
            builder.Services.AddScoped<IStoreRepository, StoreRepository>();
            //Registering the Address repository
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            //Registering the Brand repository
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            //Registering the StoreBrand repository
            builder.Services.AddScoped<IStoreBrandRepository, StoreBrandRepository>();
            //Registering the Product repository
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
        }

        //Registering the BLL services
        private static void RegisterBllServices(WebApplicationBuilder builder)
        {
            //Adding the User Service to the DI Container
            builder.Services.AddScoped<IUserService, UserService>();
            //Adding the Store Service to the DI Container
            builder.Services.AddScoped<IStoreService, StoreService>();
            //Adding the Address Service to the DI Container
            builder.Services.AddScoped<IAddressService, AddressService>();
            //Adding the Brand Service to the DI Container
            builder.Services.AddScoped<IBrandService, BrandService>();
            //Adding the Brand Service to the DI Container
            builder.Services.AddScoped<IProductService, ProductService>();
        }

        //Registering the helper services
        private static void RegisterHelperServices(WebApplicationBuilder builder)
        {
            //Adding the password helper to the DI Container
            builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();

            //Adding the JWT helper to the DI Container
            builder.Services.AddScoped<IJwtHelper, JWTHelper>();
        }

        //Register the Mail Services
        private static void RegisterSmptService(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IEmailService, SmtpEmailService>();
        }

        //Register Caching service
        private static void RegisterCachingService(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheManager, MemoryCacheManager>();
        }


        //This is used to configure swagger; This filter will apply the lock icon only to endpoints with [Authorize].
        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                // Check if the method explicitly has [AllowAnonymous]
                bool isAnonymous = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>().Any();

                // Check if the method OR controller has [Authorize], but not if AllowAnonymous is present
                bool requiresAuth = !isAnonymous && (
                    context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                        .OfType<AuthorizeAttribute>().Any() == true ||
                    context.MethodInfo.GetCustomAttributes(true)
                        .OfType<AuthorizeAttribute>().Any()
                );

                if (requiresAuth)
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
                }
            }
        }
    }
}
