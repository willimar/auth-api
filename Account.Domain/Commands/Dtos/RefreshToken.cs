using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Commands.Dtos
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
    }
}
