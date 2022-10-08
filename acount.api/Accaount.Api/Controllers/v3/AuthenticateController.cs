using Account.Domain.Commands;
using Account.Domain.Commands.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers.v3
{
    [EnableCors(SwaggerSetup.AllowAnyOrigins)]
    [ApiExplorerSettings(GroupName = "Authenticate")]
    [ApiVersion("3", Deprecated = false)]
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]/")]
    public class AuthenticateController: ControllerBase
    {
        private readonly AuthenticateCommand _authenticateCommand;

        public AuthenticateController(AuthenticateCommand authenticateCommand)
        {
            this._authenticateCommand = authenticateCommand;
        }

        [HttpPost("authenticate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Authenticate([FromBody] AutenticateUser autenticateUser)
        {
            if (autenticateUser == null)
            {
                return BadRequest();
            }

            var response = await this._authenticateCommand.Authenticate(autenticateUser);

            if (response.isValid)
            {
                this.Response.Headers.Authorization = $"Bearer {response.token}";
                this.Response.Headers.ProxyAuthorization = $"Bearer {response.refreshToken}";

                return await ValueTask.FromResult(Ok());
            }
            else
            {
                this.Response.Headers.Authorization = $"Bearer {string.Empty}";
                this.Response.Headers.ProxyAuthorization = $"Bearer {string.Empty}";

                return await ValueTask.FromResult(Unauthorized());
            }
        }

        [HttpPost("refresh-token")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async ValueTask<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            var response = await this._authenticateCommand.Authenticate(refreshToken.Token);

            if (response.isValid)
            {
                this.Response.Headers.Authorization = $"Bearer {response.token}";
                this.Response.Headers.ProxyAuthorization = $"Bearer {response.refreshToken}";

                return await ValueTask.FromResult(Ok());
            }
            else
            {
                this.Response.Headers.Authorization = $"Bearer {string.Empty}";
                this.Response.Headers.ProxyAuthorization = $"Bearer {string.Empty}";

                return await ValueTask.FromResult(Unauthorized());
            }

            return await ValueTask.FromResult(Ok());
        }
    }
}
