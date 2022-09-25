using Account.Domain.Entities;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Validators
{
    public class AppendUserValidator : IValidator<User>
    {
        public ValueTask<IValidatorResult> Validate(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
