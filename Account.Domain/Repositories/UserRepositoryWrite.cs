using Account.Domain.Entities;
using DataCore.Domain.Abstract;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Repositories
{
    internal class UserRepositoryWrite : RepositoryWriteBase<User>
    {
        public UserRepositoryWrite(IDataProviderWrite provider, IUser user) : base(provider, user)
        {
        }
    }
    internal class UserRepositoryRead : RepositoryReadBase<User>
    {
        public UserRepositoryRead(IDataProviderRead provider) : base(provider)
        {
        }
    }
}
