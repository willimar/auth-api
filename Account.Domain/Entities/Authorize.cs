using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;

namespace Account.Domain.Entities
{
    public class Authorize : IEntity
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public StatusRecord Status { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
