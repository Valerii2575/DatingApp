using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Extensions
{
    public static class AuthenticationServiceExtentions
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services, 
                IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(options => {
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
    }
}