using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using DataCore.Domain.Interfaces;
using System.Xml.Linq;

namespace Account.Domain.Queries
{
    public class AuthorizeQuery
    {
        private readonly IRepositoryRead<Authorize> _repositoryAuthorize;

        public AuthorizeQuery(IRepositoryRead<Authorize> repositoryAuthorize) 
        {
            this._repositoryAuthorize = repositoryAuthorize;
        }

        public async ValueTask<List<ChangeAuthorize>> GetAuthorizations()
        {
            var response = this._repositoryAuthorize.GetData(x => x.Status == DataCore.Domain.Enumerators.StatusRecord.Active, null).Select(x => GetItem(x)).ToList();

            return await ValueTask.FromResult(response);
        }

        private static ChangeAuthorize GetItem(Authorize item)
        {
            ChangeAuthorize response = new() 
            { 
                Description = item.Description,
                Name = item.Name,
            };

            return response;
        }
    }
}
