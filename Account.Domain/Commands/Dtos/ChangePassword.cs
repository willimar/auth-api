using System.Text.Json.Serialization;

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
