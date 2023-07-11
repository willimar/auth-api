using Account.Api.Setups;
using Consul;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Api.Extensions
{
    public static class ServiceRegistreExtensions
    {
        public static IServiceCollection AddConsulSettings(this IServiceCollection services, ServiceSettings serviceSettings)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config => {
                config.Address = new Uri(serviceSettings.ServiceDiscoveryAddres);
            }));

            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, ServiceSettings serviceSettings)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifeTime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var registration = new AgentServiceRegistration()
            {
                ID = serviceSettings.ServiceName,
                Name = serviceSettings.ServiceName,
                Address = serviceSettings.ServiceHost,
                Port = serviceSettings.ServicePort,
            };

            logger.LogInformation("Registering with Consul.");

            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifeTime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistrering from Consul.");
            });

            return app;
        }
    }
}
