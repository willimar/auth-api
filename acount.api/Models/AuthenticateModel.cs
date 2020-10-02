using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acount.api.Models
{
    public class AuthenticateModel
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}
