namespace Account.Domain.Commands.Dtos
{
    public class AppendUser : AppendAccount
    {
        public Guid TenantId { get; set; }
        public Guid GroupId { get; set; }
    }
}
