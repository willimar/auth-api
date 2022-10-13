using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Account.Domain.Commands.Dtos
{
    public class ChangePassword
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        [JsonIgnore]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Guid TenantId { get; set; }
        [JsonIgnore]
        public Guid GroupId { get; set; }
    }
}
