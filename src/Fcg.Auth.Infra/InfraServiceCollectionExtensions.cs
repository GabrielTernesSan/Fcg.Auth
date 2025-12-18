using Fcg.Auth.Application.Services;
using Fcg.Auth.Domain.Queries;
using Fcg.Auth.Domain.Repositories;
using Fcg.Auth.Domain.Services;
using Fcg.Auth.Infra.Queries;
using Fcg.Auth.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Auth.Infra
{
    public static class InfraServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FcgAuthDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAuthUserRepository, AuthUserRepository>();
            services.AddScoped<IAuthQuery, AuthQuery>();

            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthUserRepository, AuthUserRepository>();

            return services;
        }
    }
}
