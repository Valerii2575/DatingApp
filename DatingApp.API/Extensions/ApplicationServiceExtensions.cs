using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => {
                        options.UseSqlite(config.GetConnectionString("DefaultConnection"));
                    });

            services.AddCors();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            return services;
        }

        public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPhotoService, PhotoService>();

            return services;
        }
    }
}