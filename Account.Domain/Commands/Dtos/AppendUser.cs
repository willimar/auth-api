using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Commands.Dtos
{
    public class AppendUser: AppendAccount
    {
        public Guid TenantId { get; set; }
        public Guid GroupId { get; set; }
    }
}
