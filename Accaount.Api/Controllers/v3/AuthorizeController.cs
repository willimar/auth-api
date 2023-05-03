using Account.Domain.Commands;
using Account.Domain.Commands.Dtos;
using Account.Domain.Queries;
using Account.Domain.Validators;
using DataCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using System.Text.Json;

namespace Account.Api.Controllers.v3
{
    [EnableCors(SwaggerSetup.AllowAnyOrigins)]
    [ApiExplorerSettings(GroupName = "Athorization")]
    [ApiVersion("3", Deprecated = false)]
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]/")]
    [IgnoreAntiforgeryToken(Order = 1001)]
    [Authorize]
    public class AuthorizeController : ControllerBase
    {
        private readonly AuthorizateCommand _authorizateCommand;
        private readonly AuthorizeQuery _authorizeQuery;

        public AuthorizeController(AuthorizateCommand authorizateCommand, AuthorizeQuery authorizeQuery)
        {
            this._authorizateCommand = authorizateCommand;
            this._authorizeQuery = authorizeQuery;
        }

        [HttpPost("register-authorization")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async ValueTask<IActionResult> RegisterAuthorize([FromBody] AppendAuthorize appendAuthorize)
        {
            try
            {
                var response = await this._authorizateCommand.Append<AppendAuthorizateValidator>(appendAuthorize, new CancellationToken());

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

        [HttpPut("change-authorization")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async ValueTask<IActionResult> ChangeAuthorize([FromBody] ChangeAuthorize appendAuthorize)
        {
            try
            {
                var response = await this._authorizateCommand.Change<ChangeAuthorizateValidator>(appendAuthorize, new CancellationToken());

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

        [HttpDelete("remove-authorization")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async ValueTask<IActionResult> RemoveAuthorize([FromBody] RemoveAuthorize appendAuthorize)
        {
            try
            {
                var response = await this._authorizateCommand.Remove(appendAuthorize, new CancellationToken());

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

        [HttpGet("get-authorizations")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async ValueTask<IActionResult> GetAuthorize()
        {
            try
            {
                var response = await this._authorizeQuery.GetAuthorizations();

                if (!response.Any())
                {
                    return await ValueTask.FromResult(StatusCode(204));
                }

                return await ValueTask.FromResult(StatusCode(200, response));
            }
            catch (Exception e)
            {
                return await ValueTask.FromResult(StatusCode(500, e.Message));
            }
        }
    }
}
