using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using Account.Domain.Mappers;
using Account.Domain.Properties;
using Auvo.Financeiro.Application.Mappers.Fornecedor;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Account.Domain.Commands
{
    public class AuthorizateCommand
    {
        private readonly IRepositoryWrite<Authorize> _repositoryWrite;
        private readonly IRepositoryRead<Authorize> _repositoryRead;
        private readonly AuthorizeMapperConfig _authorizeMapperConfig;
        private readonly IServiceProvider _service;

        public AuthorizateCommand(IRepositoryWrite<Authorize> repositoryWrite, IRepositoryRead<Authorize> repositoryRead, AuthorizeMapperConfig authorizeMapperConfig, IServiceProvider service)
        {
            this._repositoryWrite = repositoryWrite;
            this._repositoryRead = repositoryRead;
            this._authorizeMapperConfig = authorizeMapperConfig;
            this._service = service;
        }

        public async ValueTask<IEnumerable<IHandleMessage>?> Append<TValidator>(AppendAuthorize append, CancellationToken cancellationToken) where TValidator : class, IValidator<Authorize>
        {
            var authorize = this._authorizeMapperConfig.CreateMapper().Map<Authorize>(append);
            var validator = this._service.GetService(typeof(TValidator)) as TValidator;

            var response = (await this._repositoryWrite.AppenData(authorize, validator, cancellationToken))?.ToList();

            response ??= new List<IHandleMessage>();

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._repositoryWrite.ContextApplyChanges(cancellationToken);

                if (response.Any(x => x.Code == HandlesCode.BadRequest) || !saved)
                {
                    response.Add(HandleMessage.Factory("AppendAuthorizationError", "Insert record error to new authorization.", HandlesCode.InternalException));
                }
                else
                {
                    response.Add(HandleMessage.Factory("AppendAuthorization", "Inserted record to new authorization.", HandlesCode.Accepted));
                }
            }

            return response;
        }

        public async ValueTask<IEnumerable<IHandleMessage>?> Change<TValidator>(ChangeAuthorize changeAuthorize, CancellationToken cancellationToken) where TValidator : class, IValidator<Authorize>
        {
            var authorize = this._repositoryRead.GetData(x => x.Name.ToLower() == changeAuthorize.Name.ToLower()).FirstOrDefault();
            authorize = this._authorizeMapperConfig.CreateMapper().Map(changeAuthorize, authorize);
            IEnumerable<IHandleMessage> response = new List<IHandleMessage>();

            if (authorize is null)
            {
                response = response.Append(HandleMessage.Factory(nameof(Resources.RegisterNotFoundException), Resources.RegisterNotFoundException, HandlesCode.ValueNotFound));
                return response;
            }

            var validator = this._service.GetService(typeof(TValidator)) as TValidator;
            response = await this._repositoryWrite.UpdateData(authorize, validator, cancellationToken);

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._repositoryWrite.ContextApplyChanges(cancellationToken);

                if (response.Any(x => x.Code == HandlesCode.BadRequest) || !saved)
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.ChangeRecordError), Resources.ChangeRecordError, HandlesCode.InternalException));
                }
                else
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.RecordChanged), Resources.RecordChanged, HandlesCode.Accepted));
                }
            }
            return response;
        }

        public async ValueTask<IEnumerable<IHandleMessage>?> Remove(RemoveAuthorize removeAuthorize, CancellationToken cancellationToken)
        {
            var authorize = this._repositoryRead.GetData(x => x.Name.ToLower() == removeAuthorize.Name.ToLower()).FirstOrDefault();

            IEnumerable<IHandleMessage> response = new List<IHandleMessage>();

            if (authorize is null)
            {
                response = response.Append(HandleMessage.Factory(nameof(Resources.RegisterNotFoundException), Resources.RegisterNotFoundException, HandlesCode.ValueNotFound));
                return response;
            }

            response = await this._repositoryWrite.DeleteData(authorize, null, cancellationToken);

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._repositoryWrite.ContextApplyChanges(cancellationToken);

                if (response.Any(x => x.Code == HandlesCode.BadRequest) || !saved)
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.ChangeRecordError), Resources.ChangeRecordError, HandlesCode.InternalException));
                }
                else
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.RecordChanged), Resources.RecordChanged, HandlesCode.Accepted));
                }
            }
            return response;
        }
    }
}
