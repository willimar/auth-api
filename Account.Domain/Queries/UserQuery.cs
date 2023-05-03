using Account.Domain.Entities;
using DataCore.Domain.Interfaces;

namespace Account.Domain.Queries
{
    public class UserQuery
    {
        private readonly IRepositoryRead<User> _repositoryUser;

        public UserQuery(IRepositoryRead<User> repositoryUser)
        {
            this._repositoryUser = repositoryUser;
        }

        public async ValueTask<bool> IsValid(IEnumerable<string> argumentValues)
        {
            var hashId = argumentValues.ToList().ComputeMd5Hash();
            var response = this._repositoryUser.GetData(x => x.UserHashes.Any(r => r.Value.Equals(hashId)), null).Any();

            return await ValueTask.FromResult(response);
        }
    }
}
