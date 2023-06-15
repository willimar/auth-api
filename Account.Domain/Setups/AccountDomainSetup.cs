using Account.Domain.Commands;
using Account.Domain.Entities;
using Account.Domain.Mappers;
using Account.Domain.Queries;
using Account.Domain.Repositories;
using Account.Domain.Validators;
using DataCore.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Domain.Setups
{
    public static class AccountDomainSetup
    {
        public static void RegisterAccountDomain(this IServiceCollection services)
        {
            services.AddScoped<UserCommand>();
            services.AddScoped<AuthorizateCommand>();
            services.AddScoped<AuthenticateCommand>();

            services.AddScoped<IRepositoryWrite<User>, UserRepositoryWrite>();
            services.AddScoped<IRepositoryWrite<Authorize>, AuthorizeRepositoryWrite>();

            services.AddScoped<IRepositoryRead<User>, UserRepositoryRead>();
            services.AddScoped<IRepositoryRead<Authorize>, AuthorizeRepositoryRead>();

            services.AddScoped<AppendAuthorizeMapper>();
            services.AddScoped<ChangeAuthorizeMapper>();

            services.AddScoped<AppendAccountAppendUserMapper>();
            services.AddScoped<AppendUserMapper>();
            services.AddScoped<AppendAccountMapper>();

            services.AddScoped<AppendAccountValidator>();
            services.AddScoped<AppendAuthorizateValidator>();
            services.AddScoped<ChangeAuthorizateValidator>();

            services.AddScoped<AuthorizeQuery>();
        }
    }
}
