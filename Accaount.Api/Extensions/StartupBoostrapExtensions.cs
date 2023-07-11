using Account.Api.Setups;
using Microsoft.Extensions.Options;

namespace Account.Api.Extensions
{
    public static class StartupBoostrapExtensions
    {
        public static ServiceSettings StartupBoosttrap(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceSettings>(configuration.GetSection(nameof(ServiceSettings)));
            var serviceProvider = services.BuildServiceProvider();
            var iop = serviceProvider.GetService<IOptions<ServiceSettings>>();

            return iop?.Value ?? throw new NullReferenceException(nameof(ServiceSettings));
        }
    }
}
