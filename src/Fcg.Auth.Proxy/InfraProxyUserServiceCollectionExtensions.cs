using Fcg.Auth.Proxy.User.Client;
using Fcg.Auth.Proxy.User.Client.Interface;
using Fcg.Auth.Proxy.User.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Fcg.Auth.Proxy.User
{
    public static class InfraProxyUserServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraProxyUser(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IClientUser, ClientUser>();

            services.AddHttpClient<IClientUser, ClientUser>();

            services.Configure<UserConfiguration>(configuration.GetSection("UserConfiguration"));
            services.TryAddSingleton<IValidateOptions<UserConfiguration>, UserConfigurationValidation>();

            return services;
        }
    }
}
