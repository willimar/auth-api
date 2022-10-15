using Account.Api.Extensions;
using Account.Domain.Setups;
using DataCore.Domain.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Swagger.Simplify;
using System.Globalization;
using System.Reflection;

namespace Account.Api
{
    public static class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiInfo = ApiInfo.Factory();

            Startup.SetApiInfo(apiInfo.Info);

            apiInfo.MajorVersion = 3;
            apiInfo.GroupNameFormat = "'v'V";

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllersWithViews();
            services.AddRouting();
            services.GenerateToVersion(apiInfo);
            services.PrepareAnyCors();
            services.RegisterAccountDomain();
            services.RegisterUserContext(configuration);

            services.AddScoped<IDataProviderWrite>(serviceProvider => serviceProvider.GetDataProviderWrite(configuration));
            services.AddScoped<IDataProviderRead>(serviceProvider => serviceProvider.GetDataProviderReader(configuration));
        }

        private static (AssemblyDescriptionAttribute? descriptionAttribute, AssemblyProductAttribute? productAttribute, AssemblyCopyrightAttribute? copyrightAttribute, AssemblyName assemblyName) GetAssemblyInfo()
        {
            var assembly = typeof(Program).Assembly;
            var assemblyInfo = assembly.GetName();

            var descriptionAttribute = assembly
                 .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                 .OfType<AssemblyDescriptionAttribute>()
                 .FirstOrDefault();
            var productAttribute = assembly
                 .GetCustomAttributes(typeof(AssemblyProductAttribute), false)
                 .OfType<AssemblyProductAttribute>()
                 .FirstOrDefault();
            var copyrightAttribute = assembly
                 .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                 .OfType<AssemblyCopyrightAttribute>()
                 .FirstOrDefault();

            return (descriptionAttribute, productAttribute, copyrightAttribute, assemblyInfo);
        }

        private static void SetApiInfo(OpenApiInfo info)
        {
            var (descriptionAttribute, productAttribute, copyrightAttribute, assemblyName) = GetAssemblyInfo();

            info.Title = productAttribute?.Product;
            info.Version = assemblyName.Version?.ToString();
            info.Description = descriptionAttribute?.Description;
            info.Contact = new OpenApiContact
            {
                Name = copyrightAttribute?.Copyright,
                Url = new Uri(@"https://github.com/willimar"),
                Email = "willimar@gmail.com",
            };
            info.TermsOfService = null;
            info.License = new OpenApiLicense()
            {
                Name = "RESTRICT USE LICENSE",
                Url = new Uri(@"https://github.com/willimar")
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var suportedCultures = new CultureInfo[] { currentCulture };

            app.UseMiddleware<AccountMiddleware>();
            app.ConfigureInSwaggerSimplify(currentCulture, suportedCultures, typeof(Startup).Assembly);
        }
    }
}
