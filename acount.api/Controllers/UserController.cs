using crud.api.core;
using crud.api.core.enums;
using crud.api.core.interfaces;
using crud.api.register.validations.register;
using FluentValidation;
using FluentValidation.Validators;
using jwt.simplify.services;
using Jwt.Simplify.Core.Entities;
using Jwt.Simplify.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace acount.api.Controllers
{
    [EnableCors(Program.AllowSpecificOrigins)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController: Controller
    {
        private readonly UserService _userService;
        private readonly UserValidator<User> _userValidator;

        public UserController(UserService userService, UserValidator<User> userValidator)
        {
            this._userService = userService;
            this._userValidator = userValidator;
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> Save(User user)
        {
            var userUri = new Uri(Program.AthenticateApi, "api/Authorized");
            var auth = Request.Authenticated(userUri.ToString());

            if (!auth.HasAccess(RulerType.EditorUser, "User"))
            {
                var error = new List<IHandleMessage>() { new HandleMessage("AccessDanied", "Sorry but you haven't permission to access this service.", HandlesCode.ManyRecordsFound) };
                return StatusCode((int)HttpStatusCode.Unauthorized, error);
            }

            var accountId = auth.GetAccountId(Request.GetSystemName());

            if (accountId == Guid.Empty)
            {
                var error = new List<IHandleMessage>() { new HandleMessage("AccessDanied", "Sorry but you haven't permission to access this service.", HandlesCode.ManyRecordsFound) };
                return StatusCode((int)HttpStatusCode.Unauthorized, error);
            }

            #region Validation Setup
            this._userValidator.RuleFor(x => x.Email)
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("Invalid value to e-mail address.");
            this._userValidator.RuleFor(x => x.Login)
                .NotEmpty()
                .NotNull()
                .Length(8, 16)
                .WithMessage("The login needs be between 8 and 16 characters.");
            this._userValidator.RuleFor(x => x.HashIdEmail)
                .NotEmpty()
                .NotNull()
                .WithMessage("Passowrdo not set to user.");
            this._userValidator.RuleFor(x => x.HashIdLogin)
                .NotEmpty()
                .NotNull()
                .WithMessage("Passowrdo not set to user.");
            this._userValidator.RuleFor(x => x.Roles)
                .NotEmpty()
                .NotNull()
                .WithMessage("Not seted rulers to user. invalid user configuration.");
            this._userValidator.RuleFor(x => x.AuthorizedSystems)
                .NotEmpty()
                .NotNull()
                .WithMessage("Not seted systems to user. invalid user configuration.");
            #endregion

            var validations = this.Validate(this._userValidator, user);

            if (validations.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, validations);
            }

            var dataUser = this._userService.GetData(x => x.Email == user.Email || x.Login == user.Login).FirstOrDefault();

            if (dataUser != null)
            {
                if (!dataUser.Id.Equals(user.Id))
                {
                    var error = new List<IHandleMessage>() { new HandleMessage("InvalidRecord", "There is other user with this login or e-mail.", HandlesCode.ManyRecordsFound) };
                    return StatusCode((int)HttpStatusCode.BadRequest, error);
                }
            }

            if (!user.AuthorizedSystems.Any(x => x.AccountId == accountId))
            {
                user.AuthorizedSystems.Add(new AuthorizedSystem() {
                    AccountId = accountId,
                    SystemName = Request.GetSystemName()
                });
            }

            var result = this._userService.SaveData(user);

            return StatusCode((int)HttpStatusCode.OK, result);
        }

        private IEnumerable<IHandleMessage> Validate(UserValidator<User> validator, User user)
        {
            var validations = validator.Validate(user);

            if (validations.IsValid)
            {
                return user.Validate();
            }

            var result = new List<IHandleMessage>(validations.Errors.Count);
            foreach (var item in validations.Errors)
            {
                result.Add(new HandleMessage(item.PropertyName, item.ErrorMessage, HandlesCode.InvalidField));
            }

            return result;
        }
    }
}
