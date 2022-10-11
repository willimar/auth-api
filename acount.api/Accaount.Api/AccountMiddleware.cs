using Microsoft.AspNetCore.Authentication;

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
            var authenticateInfo = await httpContext.AuthenticateAsync("Bearer");
            var bearerTokenIdentity = authenticateInfo?.Principal;

            if (bearerTokenIdentity != null)
            {
                httpContext.User = bearerTokenIdentity;
            }

            await _next(httpContext);
        }
    }
}
