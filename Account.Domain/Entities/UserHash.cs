using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entities
{
    public class UserHash: IEntity
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public StatusRecord Status { get; set; } = StatusRecord.Active;
        public Guid UserId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
