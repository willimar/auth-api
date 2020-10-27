using account.application.Models;
using crud.api.core;
using crud.api.core.enums;
using crud.api.core.fieldType;
using crud.api.core.interfaces;
using crud.api.core.mappers;
using crud.api.core.services;
using crud.api.dto.Person;
using crud.api.register.entities.registers;
using crud.api.register.validations;
using FluentValidation;
using FluentValidation.Validators;
using jwt.simplify.services;
using Jwt.Simplify.Core.Entities;
using Jwt.Simplify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace account.application
{
    public class AccountApplication
    {
        private readonly IService<Person<User>> _personService;
        private readonly MapperProfile<PersonModel, Person<User>> _userProfile;
        private readonly UserService _userService;

        public string SystemName { get; set; }
        public bool HasAccess { get; set; }
        public Guid AccountId { get; set; }

        public AccountApplication(IService<Person<User>> personService,
            MapperProfile<PersonModel, Person<User>> userProfile,
            UserService userService)
        {
            this._personService = personService;
            this._userProfile = userProfile;
            this._userService = userService;
        }

        private Person<User> GetPerson(IService<Person<User>> personService, 
            MapperProfile<PersonModel, Person<User>> userProfile, 
            PersonModel value)
        {
            var entity = this._personService.GetData(e =>
                    (e.User.Login.Equals(value.UserInfo.UserName) ||
                    e.User.Email.Equals(value.UserInfo.UserEmail))
                ).FirstOrDefault();

            // Only one account by email or username
            if (entity != null)
            {
                return null;
            }

            entity = userProfile.Map(value);

            entity.Id = Guid.NewGuid();
            entity.AccountId = entity.Id;

            return entity;
        }

        private User GetUser(Person<User> entity)
        {
            var user = entity.User;

            user.Id = Guid.NewGuid();
            user.Roles = GetAccountRulers();
            user.AuthorizedSystems = GetAccountSystem(entity.Id);

            return user;
        }

        private IEnumerable<IHandleMessage> Validate(BaseValidator<User> validator, User user)
        {
            var validations = validator.Validate(user);
            var result = new List<IHandleMessage>(validations.Errors.Count);
            foreach (var item in validations.Errors)
            {
                result.Add(new HandleMessage(item.PropertyName, item.ErrorMessage, HandlesCode.InvalidField));
            }

            return result;
        }

        private List<AuthorizedSystem> GetAccountSystem(Guid accountId)
        {
            if (string.IsNullOrEmpty(this.SystemName))
            {
                throw new ArgumentNullException(nameof(this.SystemName));
            }

            return new List<AuthorizedSystem>()
                {
                    new AuthorizedSystem()
                    {
                        AccountId = accountId,
                        SystemName = this.SystemName
                    }
                };
        }

        private List<UserRule> GetAccountRulers()
        {
            return new List<UserRule>()
                {
                    new UserRule()
                    {
                        RolerName = "Root",
                        Id = Guid.NewGuid(),
                        LastChangeDate = DateTime.UtcNow,
                        RegisterDate = DateTime.UtcNow,
                        Roler = RulerType.SuperUser,
                        Status = RecordStatus.Active
                    }
                };
        }

        private BaseValidator<User> UserValidatorConfigure()
        {
            var validator = new BaseValidator<User>();
            #region Validation Setup
            validator.RuleFor(x => x.Email)
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("Invalid value to e-mail address.");
            validator.RuleFor(x => x.Login)
                .NotEmpty()
                .NotNull()
                .Length(8, 16)
                .WithMessage("The login needs be between 8 and 16 characters.");
            validator.RuleFor(x => x.Roles)
                .NotEmpty()
                .NotNull()
                .WithMessage("Not seted rulers to user. invalid user configuration.");
            validator.RuleFor(x => x.AuthorizedSystems)
                .NotEmpty()
                .NotNull()
                .WithMessage("Not seted systems to user. invalid user configuration.");
            #endregion

            return validator;
        }

        public IEnumerable<IHandleMessage> CreateAccount(PersonModel value)
        { 
            List<IHandleMessage> handleMessage = new List<IHandleMessage>();

            try
            {
                var entity = this.GetPerson(this._personService, this._userProfile, value);
                
                // Only one account by email or username
                if (entity == null)
                {
                    handleMessage.Add(new HandleMessage("ThereIsAccount", $"Account with this e-mail or user name was found.", HandlesCode.ManyRecordsFound));
                    return handleMessage;
                }

                var user = this.GetUser(entity);

                if (user == null)
                {
                    handleMessage.Add(new HandleMessage("ThereIsUser", $"User with this e-mail or user name was found.", HandlesCode.ManyRecordsFound));
                    return handleMessage;
                }

                var validate = entity.Validate();

                if (validate.Any())
                {
                    return validate;
                }

                this.AccountId = entity.Id;
                this.HasAccess = true;

                handleMessage.AddRange(this._personService.SaveData(entity));

                if (handleMessage.Any(x => x.Code != HandlesCode.Accepted))
                {
                    return handleMessage.Distinct();
                }

                handleMessage.AddRange(this.SaveUser(user, value.UserInfo.UserPassword));

                if (handleMessage.Any(x => x.Code != HandlesCode.Accepted))
                {
                    this._personService.DeleteData(entity);
                }

                return handleMessage.Distinct();
            }
            catch (Exception e)
            {
                handleMessage.Add(new HandleMessage(e));
                return handleMessage;
            }
        }

        public IEnumerable<IHandleMessage> Login(AuthenticateModel value)
        {
            List<IHandleMessage> handleMessage = new List<IHandleMessage>();

            try
            {
                handleMessage.Add(this._userService.Login(value.User, value.Password));

                return handleMessage;
            }
            catch (Exception e)
            {
                handleMessage.Add(new HandleMessage(e));
                return handleMessage;
            }
        }

        public IEnumerable<IHandleMessage> SaveUser(User user, string password)
        {
            List<IHandleMessage> handleMessage = new List<IHandleMessage>();

            try
            {
                if (!this.HasAccess)
                {
                    var error = new List<IHandleMessage>() { new HandleMessage("AccessDanied", "Sorry but you haven't permission to access this service.", HandlesCode.ManyRecordsFound) };
                    return error;
                }

                if (this.AccountId == Guid.Empty)
                {
                    var error = new List<IHandleMessage>() { new HandleMessage("AccessDanied", "Sorry but you need define the Account System.", HandlesCode.ManyRecordsFound) };
                    return error;
                }

                var validator = this.UserValidatorConfigure();
                var validations = this.Validate(validator, user);

                if (validations.Any())
                {
                    return validations;
                }

                var dataUser = this._userService.GetData(x => x.Email == user.Email || x.Login == user.Login).FirstOrDefault();

                if (dataUser != null && !dataUser.Id.Equals(user.Id))
                {
                    var error = new List<IHandleMessage>() { new HandleMessage("InvalidRecord", "There is other user with this login or e-mail.", HandlesCode.ManyRecordsFound) };
                    return error;
                }

                if (!user.AuthorizedSystems.Any(x => x.AccountId == this.AccountId && x.SystemName == this.SystemName))
                {
                    user.AuthorizedSystems.Add(new AuthorizedSystem()
                    {
                        AccountId = this.AccountId,
                        SystemName = this.SystemName
                    });
                }

                handleMessage.AddRange(this._userService.SaveData(user, password));

                return handleMessage;
            }
            catch (Exception e)
            {
                handleMessage.Add(new HandleMessage(e));
                return handleMessage;
            }
        }
    }
}
