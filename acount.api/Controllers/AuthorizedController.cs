using crud.api.core;
using crud.api.core.enums;
using crud.api.core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace acount.api.Controllers
{
    [EnableCors(Program.AllowSpecificOrigins)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorizedController: Controller
    {
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public ActionResult<List<IHandleMessage>> Get()
        {
            var result = new List<IHandleMessage>() {
                new HandleMessage("UserName", User.Identity.Name, HandlesCode.Accepted),
                new HandleMessage("IsAuthenticated", User.Identity.IsAuthenticated.ToString(), User.Identity.IsAuthenticated ? HandlesCode.Accepted : HandlesCode.InvalidField)
             };

            foreach (var claim in User.Claims)
            {
                result.Add(new HandleMessage(claim.Type, claim.Value, HandlesCode.Accepted));
            }

            return result;
        }
    }
}
