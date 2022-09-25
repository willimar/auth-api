using Account.Domain.Commands;
using Account.Domain.Entities;
using Account.Domain.Mappers;
using Account.Domain.Repositories;
using Account.Domain.Validators;
using Auvo.Financeiro.Application.Mappers.Fornecedor;
using DataCore.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Setups
{
    public static class AccountDomainSetup
    {
        public static void RegisterAccountDomain(this IServiceCollection services)
        {
            services.AddScoped<UserCommand>();
            services.AddScoped<IRepositoryWrite<User>, UserRepositoryWrite>();
            services.AddScoped<IRepositoryRead<User>, UserRepositoryRead>();
            services.AddScoped<UserMapperConfig>();
            services.AddScoped<UserMapper>();
            services.AddScoped<AppendAccountValidator>();
        }
    }
}
