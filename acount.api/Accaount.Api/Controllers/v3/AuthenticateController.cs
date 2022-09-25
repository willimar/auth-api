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
        [HttpHead("authenticate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Authenticate()
        {
            return await ValueTask.FromResult(Ok());
        }

        [HttpHead("refresh-token")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> RefreshToken()
        {
            return await ValueTask.FromResult(Ok());
        }
    }
}
