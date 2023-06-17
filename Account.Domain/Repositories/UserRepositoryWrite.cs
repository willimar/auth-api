using Account.Domain.Entities;
using DataCore.Domain.Abstract;
using DataCore.Domain.Interfaces;

namespace Account.Domain.Repositories
{
    internal class UserRepositoryWrite : RepositoryWriteBase<User>
    {
        public UserRepositoryWrite(IDataProviderWrite provider, IUser user) : base(provider, user)
        {
        }
    }

    internal class AuthorizeRepositoryWrite : RepositoryWriteBase<Authorize>
    {
        public AuthorizeRepositoryWrite(IDataProviderWrite provider, IUser user) : base(provider, user)
        {
        }
    }

    internal class UserRepositoryRead : RepositoryReadBase<User>
    {
        public UserRepositoryRead(IDataProviderRead provider) : base(provider)
        {
        }
    }

    internal class AuthorizeRepositoryRead : RepositoryReadBase<Authorize>
    {
        public AuthorizeRepositoryRead(IDataProviderRead provider) : base(provider)
        {
        }
    }
}
