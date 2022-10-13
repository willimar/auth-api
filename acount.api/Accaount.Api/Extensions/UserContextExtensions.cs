using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using DataCore.Jwt.Extensions;
using DataCore.Jwt.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Account.Api.Extensions
{
    public static class UserContextExtensions
    {
        private class AuthenticatedUser : IUser
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public AuthenticatedUser(IHttpContextAccessor httpContextAccessor, IJwtOptions jwtOptions)
            {
                this._httpContextAccessor = httpContextAccessor;
                var authorization = this._httpContextAccessor.HttpContext?.Request.Headers.Authorization;
                var token = authorization?.ToString().Replace("Bearer ", string.Empty);

                this.MapperToken(token, jwtOptions);
            }

            public string UserName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
            public Guid Id { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
            public StatusRecord Status { get; set; }
            public Guid TenantId { get; set; }
            public Guid GroupId { get; set; }

            private void MapperToken(string? token, IJwtOptions jwtOptions)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }

                var user = token.GetUser(jwtOptions);

                if (user is null)
                {
                    return;
                }

                this.UserEmail = user.UserEmail;
                this.UserName = user.UserName;
                this.FullName = user.FullName;
                this.Id = user.Id;
                this.Created = user.Created;
                this.Modified = user.Modified;
                this.Status = user.Status;
                this.TenantId = user.TenantId;
                this.GroupId = user.GroupId;
            }
        }

        private class JwtOptions : IJwtOptions
        {
            public string? Key { get; set; }
            public double Expires { get; set; }
            public string SecurityAlgorithm { get; set; }
            public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public double ExpiresRefreshToken { get; set; }

            public JwtOptions(IConfiguration configuration)
            {
                var jwt = "JwtOptions";
                this.Key = configuration.ReadConfig<string>(jwt, "Key");
                this.Expires = configuration.ReadConfig<double>(jwt, "Expires");
                this.ExpiresRefreshToken = configuration.ReadConfig<double>(jwt, "ExpiresRefreshToken");
                this.SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
                this.Issuer = configuration.ReadConfig<string>(jwt, "Issuer");
                this.Audience = configuration.ReadConfig<string>(jwt, "Audience");
            }
        }

        public static void RegisterUserContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IJwtOptions, JwtOptions>();
            services.AddScoped<IUser, AuthenticatedUser>();
            services.AddAuthentication(configuration.GetJwtOptions());
        }

        private static IJwtOptions GetJwtOptions(this IConfiguration configuration)
        {
            return new JwtOptions(configuration);
        }
    }
}
