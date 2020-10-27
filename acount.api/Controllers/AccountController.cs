using account.application;
using account.application.Models;
using crud.api.core;
using crud.api.core.interfaces;
using crud.api.dto.Person;
using Jwt.Simplify.Core.Entities;
using Jwt.Simplify.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace acount.api.Controllers
{
    [EnableCors(Program.AllowSpecificOrigins)]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AccountApplication _accountApplication;

        public AccountController(AccountApplication accountApplication)
        {
            this._accountApplication = accountApplication;
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> Append(PersonModel value)
        {
            try
            {
                this._accountApplication.SystemName = Request.GetSystemName();
                var result = this._accountApplication.CreateAccount(value);

                return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }
            catch (Exception e)
            {
                List<IHandleMessage> handleMessage = new List<IHandleMessage>();
                handleMessage.Add(new HandleMessage(e));
                return StatusCode((int)HttpStatusCode.InternalServerError, handleMessage);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> Login(AuthenticateModel value)
        {
            try
            {
                var result = this._accountApplication.Login(value);

                return StatusCode((int)HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                List<IHandleMessage> handleMessage = new List<IHandleMessage>();
                handleMessage.Add(new HandleMessage(e));
                return StatusCode((int)HttpStatusCode.InternalServerError, handleMessage);
            }
        }

        [HttpPost]
        [Authorize]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> SaveUser(User user, string password)
        {
            try
            {
                var userUri = new Uri(Program.AthenticateApi, "api/Authorized");
                var auth = Request.Authenticated(userUri.ToString());

                this._accountApplication.SystemName = Request.GetSystemName();
                this._accountApplication.HasAccess = auth.HasAccess(RulerType.EditorUser, "User");
                this._accountApplication.AccountId = auth.GetAccountId(Request.GetSystemName());

                var result = this._accountApplication.SaveUser(user, password);

                return StatusCode((int)HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                List<IHandleMessage> handleMessage = new List<IHandleMessage>();
                handleMessage.Add(new HandleMessage(e));
                return StatusCode((int)HttpStatusCode.InternalServerError, handleMessage);
            }
        }
    }
}
