using Account.Domain.Commands;
using Account.Domain.Commands.Dtos;
using Account.Domain.Mappers;
using Account.Domain.Validators;
using DataCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers.v3
{
    [EnableCors(SwaggerSetup.AllowAnyOrigins)]
    [ApiExplorerSettings(GroupName = "Users")]
    [ApiVersion("3", Deprecated = false)]
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]/")]
    [IgnoreAntiforgeryToken(Order = 1001)]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserCommand _userCommand;

        public UserController(UserCommand userCommand)
        {
            this._userCommand = userCommand;
        }

        [HttpPost("register-account")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> RegisterAccount([FromBody] AppendAccount appendAccount)
        {
            try
            {
                var response = await this._userCommand.Append<AppendAccountValidator, AppendAccount, AppendAccountMapper>(appendAccount, new CancellationToken());

                response ??= new List<IHandleMessage>();
                if (response.Any(x => (int)x.Code >= 400 && (int)x.Code < 500))
                {
                    return await ValueTask.FromResult(BadRequest(response));
                }
                else if (response.Any(x => (int)x.Code >= 500 && (int)x.Code < 600))
                {
                    return await ValueTask.FromResult(StatusCode(500, response));
                }

                return await ValueTask.FromResult(Ok(response));
            }
            catch (Exception e)
            {
                return await ValueTask.FromResult(StatusCode(500, e.Message));
            }
        }

        [HttpPost("register-user")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [Authorize]
        public async ValueTask<IActionResult> RegisterUser([FromBody] AppendAccount appendAccount, [FromServices] IUser user, [FromServices] AppendAccountAppendUserMapper userMapperConfig)
        {
            try
            {
                var appendUser = userMapperConfig.Map(appendAccount);
                appendUser.TenantId = user.TenantId;
                appendUser.GroupId = user.GroupId;

                var response = await this._userCommand.Append<AppendUserValidator, AppendUser, AppendUserMapper>(appendUser, new CancellationToken());

                response ??= new List<IHandleMessage>();
                if (response.Any(x => (int)x.Code >= 400 && (int)x.Code < 500))
                {
                    return await ValueTask.FromResult(BadRequest(response));
                }
                else if (response.Any(x => (int)x.Code >= 500 && (int)x.Code < 600))
                {
                    return await ValueTask.FromResult(StatusCode(500, response));
                }

                return await ValueTask.FromResult(Ok(response));
            }
            catch (Exception e)
            {
                return await ValueTask.FromResult(StatusCode(500, e.Message));
            }
        }

        [HttpPut("change-password")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [Authorize]
        public async ValueTask<IActionResult> Change([FromBody] ChangePassword changePassword, [FromServices] IUser user)
        {
            try
            {
                changePassword.UserId = user.Id;
                changePassword.TenantId = user.TenantId;
                changePassword.GroupId = user.GroupId;

                var response = await this._userCommand.ChangePassword<ChangePasswordValidator>(changePassword, new CancellationToken());

                response ??= new List<IHandleMessage>();
                if (response.Any(x => (int)x.Code >= 400 && (int)x.Code < 500))
                {
                    return await ValueTask.FromResult(BadRequest(response));
                }
                else if (response.Any(x => (int)x.Code >= 500 && (int)x.Code < 600))
                {
                    return await ValueTask.FromResult(StatusCode(500, response));
                }

                return await ValueTask.FromResult(Ok(response));
            }
            catch (Exception e)
            {
                return await ValueTask.FromResult(StatusCode(500, e.Message));
            }
        }
    }
}
