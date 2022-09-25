using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using Auvo.Financeiro.Application.Mappers.Fornecedor;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Commands
{
    public class UserCommand
    {
        private readonly IRepositoryWrite<User> _repositoryUser;
        private readonly UserMapperConfig _userMapper;
        private readonly IServiceProvider _service;

        public UserCommand(IRepositoryWrite<User> repositoryUser, UserMapperConfig userMapper, IServiceProvider service)
        {
            this._repositoryUser = repositoryUser;
            this._userMapper = userMapper;
            this._service = service;
        }

        public async ValueTask<IEnumerable<IHandleMessage>?> Append<TValidator>(AppendAccount append, CancellationToken cancellationToken) where TValidator : class, IValidator<User>
        {
            var user = this._userMapper.CreateMapper().Map<User>(append);

            var validator = this._service.GetService(typeof(TValidator)) as TValidator;
            var response = (await this._repositoryUser.AppenData(user, validator, cancellationToken))?.ToList();

            response ??= new List<IHandleMessage>();

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._repositoryUser.ContextApplyChanges(cancellationToken);

                if (response.Any(x => x.Code == HandlesCode.BadRequest) || !saved)
                {
                    response.Add(HandleMessage.Factory("AppendAccountError", "Insert record error to new account.", HandlesCode.InternalException));
                }
                else
                {
                    response.Add(HandleMessage.Factory("AppendAccount", "Inserted record to new account.", HandlesCode.Accepted));
                }
            }

            return response;
        }
    }
}
