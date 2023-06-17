using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Account.Api
{
    public class AccountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccountMiddleware> _logger;

        public AccountMiddleware(RequestDelegate next, ILogger<AccountMiddleware> logger)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var bodyContent = string.Empty;

            try
            {
                bodyContent = await GetContent(httpContext);
                await this.SetCulture(httpContext);

                if (this.IsAnonimous(httpContext))
                {
                    await this.RequestRegister(httpContext, bodyContent, "AnonimousRequestInfo");
                    await this.SiteRequestForgeryDefender(httpContext);
                }
                else
                {
                    await this.AuthenticateSet(httpContext);
                }

                await this.RequestRegister(httpContext, bodyContent, "RequestInfo");
            }
            catch (Exception e)
            {
                var exceptionName = e.GetType().Name;
                var exceptionMessage = e.Message;
                var exceptionTrace = e.StackTrace;
                var innerExceptionMessage = e.InnerException?.Message;
                var innerExceptionTrace = e.InnerException?.StackTrace;

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.Headers[exceptionName.ToLower()] = HttpUtility.UrlEncodeUnicode(exceptionMessage);
                httpContext.Response.Headers[$"{exceptionName.ToLower()}-trace"] = HttpUtility.UrlEncodeUnicode(exceptionTrace);

                await this.ExceptionRegister(exceptionName, exceptionMessage, exceptionTrace, bodyContent, innerExceptionMessage, innerExceptionTrace);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            await _next(httpContext);
        }

        private async Task<string?> GetContent(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            using var body = new MemoryStream();
            httpContext.Request.Body.CopyTo(body);
            using var reader = new StreamReader(body, encoding: Encoding.UTF8);

            var response = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            return response;
        }

        /// <summary>
        /// Registrando log de exceções
        /// </summary>
        private async Task ExceptionRegister(
            string exceptionName,
            string exceptionMessage,
            string? exceptionTrace,
            string? bodyContent,
            string? innerExceptionMessage,
            string? innerExceptionTrace)
        {
            var errorMessage = $"[Exception]: {exceptionName} \n\r Message: {exceptionMessage} \n\r StackTrace: {exceptionTrace} \n\r Body Content: {bodyContent}";

            if (!string.IsNullOrEmpty(innerExceptionMessage))
            {
                errorMessage += $"\n\r InnerException: {innerExceptionMessage} \n\r StackTrace: {innerExceptionTrace}";
            }

            Console.Error.WriteLine(errorMessage);
            this._logger.LogError(errorMessage);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Registrando log de requisição
        /// </summary>
        private async Task RequestRegister(HttpContext httpContext, string? bodyContent, string logName)
        {
            var isPasswordInfo = bodyContent?.ToLower().ToString().Contains("password") ?? false;

            bodyContent = isPasswordInfo ? string.Empty : bodyContent;

            var obj = new
            {
                httpContext.Request.Path,
                httpContext.Request.Method,
                bodyContent,
                logName,
            };

            this._logger.LogInformation(logName, obj);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Para criar mecanismos de defesa contra ataques CSRF
        /// </summary>
        private async Task SiteRequestForgeryDefender(HttpContext httpContext)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Tentando validar se o metodo é anonimo. 
        /// </summary>
        private bool IsAnonimous(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Setando valores segundo token.
        /// </summary>
        private async Task AuthenticateSet(HttpContext httpContext)
        {
            var authenticateInfo = await httpContext.AuthenticateAsync("Bearer");
            var bearerTokenIdentity = authenticateInfo?.Principal;

            if (bearerTokenIdentity is not null)
            {
                httpContext.User ??= bearerTokenIdentity;
            }
        }

        /// <summary>
        /// Habilitando globalização para resourcers
        /// </summary>
        public async Task SetCulture(HttpContext httpContext)
        {
            var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
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
