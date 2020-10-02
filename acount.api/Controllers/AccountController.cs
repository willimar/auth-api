using acount.api.Models;
using city.core.entities;
using crud.api.core;
using crud.api.core.enums;
using crud.api.core.fieldType;
using crud.api.core.interfaces;
using crud.api.core.mappers;
using crud.api.core.repositories;
using crud.api.core.services;
using crud.api.dto.Person;
using crud.api.register.entities.registers;
using jwt.simplify.entities;
using jwt.simplify.enums;
using jwt.simplify.repositories;
using jwt.simplify.services;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly MapperProfile<PersonModel, Person<User>> _userProfile;
        private readonly IService<Person<User>> _personService;
        private readonly IRepository<City> _cityRepository;
        private readonly UserService _userService;
        //private readonly UserRepository _userRepository;

        public AccountController(IService<Person<User>> personService, 
                MapperProfile<PersonModel, 
                Person<User>> userProfile, 
                IRepository<City> city, 
                UserService userService)
        {
            this._userProfile = userProfile;
            this._personService = personService;
            this._cityRepository = city;
            this._userService = userService;
            //this._userRepository = userRepository;
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> Append(PersonModel value)
        {
            List<IHandleMessage> handleMessage = new List<IHandleMessage>();

            try
            {
                var entity = this._personService.GetData(e =>
                    e.IsRoot &&
                    (e.User.Login.Equals(value.UserInfo.UserName) ||
                    e.User.Email.Equals(value.UserInfo.UserEmail))
                ).FirstOrDefault();

                // Only one account by email or username
                if (entity != null)
                {
                    handleMessage.Add(new HandleMessage("ThereIsUser", $"Account with this e-mail or user name was found.", HandlesCode.ManyRecordsFound));
                    return StatusCode((int)HttpStatusCode.BadRequest, handleMessage);
                }

                entity = this._userProfile.Map(value);

                entity.Id = Guid.NewGuid();
                entity.IsRoot = true;
                entity.RootId = entity.Id;

                var user = entity.User;

                user.Id = entity.Id;
                user.LastChangeDate = entity.LastChangeDate;
                user.Login = value.UserInfo.UserName;
                user.Status = RecordStatus.Active;
                user.RegisterDate = entity.RegisterDate;
                user.Email = value.UserInfo.UserEmail;
                user.Roles = new List<UserRule>() { new UserRule() { 
                    RolerName = "Root", 
                    Id = entity.Id, 
                    LastChangeDate = user.LastChangeDate,
                    RegisterDate = user.RegisterDate,
                    Roler = RulerType.SuperUser,
                    Status = RecordStatus.Active
                } };

                var validate = entity.Validate();

                if (validate.Any())
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, validate);
                }

                handleMessage.AddRange(this._personService.SaveData(entity));
                this._userService.SaveData(user, value.UserInfo.UserPassword);

                return StatusCode((int)HttpStatusCode.OK, handleMessage.Distinct());
            }
            catch (Exception e)
            {
                handleMessage.Add(new HandleMessage(e));
                return StatusCode((int)HttpStatusCode.InternalServerError, handleMessage);
            }
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public ActionResult<List<IHandleMessage>> Login(AuthenticateModel value)
        {
            List<IHandleMessage> handleMessage = new List<IHandleMessage>();

            try
            {
                handleMessage.Add(this._userService.Login(value.User, value.Password));

                return StatusCode((int)HttpStatusCode.OK, handleMessage);
            }
            catch (Exception e)
            {
                handleMessage.Add(new HandleMessage(e));
                return StatusCode((int)HttpStatusCode.InternalServerError, handleMessage);
            }
        }
    }
}
