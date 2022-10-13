using Microsoft.AspNetCore.Authentication;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Account.Api
{
    public class AccountMiddleware
    {
        private readonly RequestDelegate _next;

        public AccountMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
            _ = SetCulture(acceptLanguage);

            var authenticateInfo = await httpContext.AuthenticateAsync("Bearer");
            var bearerTokenIdentity = authenticateInfo?.Principal;

            if (bearerTokenIdentity != null)
            {
                httpContext.User = bearerTokenIdentity;
            }

            await _next(httpContext);
        }

        public async Task SetCulture(string acceptLanguage)
        {
            var languages = acceptLanguage.Split(',');
            var culture = string.Empty;
            var regex = new Regex("\\w{2}-\\w{2}");

            foreach (var item in languages)
            {
                var match = regex.Match(item);

                if (match.Success)
                {
                    culture = item;
                    break;
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(culture))
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                }
            }
            catch 
            {
                // Se houver problema ao setar a cultura vai ser ignorado.
            }

            await Task.CompletedTask;
        }
    }
}
