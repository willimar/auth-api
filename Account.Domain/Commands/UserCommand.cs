using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using Account.Domain.Extensions;
using Account.Domain.Properties;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using DataCore.Mapper;

namespace Account.Domain.Commands
{
    public class UserCommand
    {
        private readonly IRepositoryWrite<User> _writeUser;
        private readonly IRepositoryRead<User> _readUser;
        private readonly IServiceProvider _service;

        public UserCommand(IRepositoryWrite<User> writeUser, IRepositoryRead<User> readUser, IServiceProvider service)
        {
            this._writeUser = writeUser;
            this._readUser = readUser;
            this._service = service;
        }

        public async ValueTask<IEnumerable<IHandleMessage>?> Append<TValidator, TCommand, TMapper>(TCommand append, CancellationToken cancellationToken)
            where TValidator : class, IValidator<User>
            where TCommand : AppendAccount
            where TMapper : class, IMapperProfile<TCommand, User>
        {
            var validator = this._service.GetService(typeof(TValidator)) as TValidator;
            var mapper = this._service.GetService(typeof(TMapper)) as TMapper;

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(TMapper));
            }

            var user = mapper.Map(append);

            var response = (await this._writeUser.AppenData(user, validator, cancellationToken))?.ToList();

            response ??= new List<IHandleMessage>();

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._writeUser.ContextApplyChanges(cancellationToken);

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

        public async ValueTask<IEnumerable<IHandleMessage>?> ChangePassword<TValidator>(ChangePassword changePassword, CancellationToken cancellationToken) where TValidator : class, IValidator<User>
        {
            var user = this._readUser.GetData(x => x.Id == changePassword.UserId && x.TenantId == changePassword.TenantId).FirstOrDefault();
            IEnumerable<IHandleMessage> response = new List<IHandleMessage>();

            if (user is null)
            {
                response = response.Append(HandleMessage.Factory(nameof(Resources.UserNotFoundException), Resources.UserNotFoundException, HandlesCode.ValueNotFound));
                return response;
            }

            var newHashId = user.GetHashTo(changePassword.NewPassword);
            var oldHashId = user.GetHashTo(changePassword.OldPassword);

            var actualHashList = user.UserHashes.Where(h => oldHashId.Any(hl => hl == h.Value) && h.Status != StatusRecord.Deleted).ToList();

            if (!actualHashList.Any())
            {
                response = response.Append(HandleMessage.Factory(nameof(Resources.InvalidPasswordException), Resources.InvalidPasswordException, HandlesCode.ValueNotFound));
                return response;
            }

            foreach (var item in user.UserHashes)
            {
                item.Status = StatusRecord.Deleted;
            }

            newHashId.ToList().ForEach(item => user.UserHashes.Add(new UserHash { Value = item }));

            var validator = this._service.GetService(typeof(TValidator)) as TValidator;
            response = await this._writeUser.UpdateData(user, validator, cancellationToken);

            if (!response.Any(x => x.Code == HandlesCode.BadRequest))
            {
                var saved = await this._writeUser.ContextApplyChanges(cancellationToken);

                if (response.Any(x => x.Code == HandlesCode.BadRequest) || !saved)
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.ChangePasswordError), Resources.ChangePasswordError, HandlesCode.InternalException));
                }
                else
                {
                    response = response.Append(HandleMessage.Factory(nameof(Resources.PasswordChanged), Resources.PasswordChanged, HandlesCode.Accepted));
                }
            }
            return response;
        }
    }
}
