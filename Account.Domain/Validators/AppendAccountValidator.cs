using Account.Domain.Entities;
using Account.Domain.Properties;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using FluentValidation;

namespace Account.Domain.Validators
{
    public class AppendAccountValidator : AbstractValidator<User>, DataCore.Domain.Interfaces.IValidator<User>
    {
        private readonly IRepositoryRead<User> _userRepository;

        public AppendAccountValidator(IRepositoryRead<User> userRepository)
        {
            this._userRepository = userRepository;

            MapperValidation();
        }

        private void MapperValidation()
        {
            RuleFor(f => f.UserName)
                .NotEmpty().MinimumLength(8).MaximumLength(25).WithMessage(Resources.UsernameLimits).WithErrorCode(HandlesCode.BadRequest.ToString());
            RuleFor(f => f.FullName)
                .NotEmpty().MinimumLength(8).MaximumLength(100).WithMessage(Resources.InvalidSizeFullName).WithErrorCode(HandlesCode.BadRequest.ToString());
            RuleFor(f => f.UserEmail)
                .NotEmpty().MinimumLength(8).MaximumLength(100).EmailAddress().WithMessage(Resources.InvalidEmail).WithErrorCode(HandlesCode.BadRequest.ToString());
            RuleFor(f => f.UserHashes)
                .NotEmpty().WithMessage(Resources.InvalidPasswordSets).WithErrorCode(HandlesCode.BadRequest.ToString())
                ;
        }

        public async ValueTask<IValidatorResult> Validate(User entity, CancellationToken cancellationToken)
        {
            var userName = entity.UserName.ToLower();
            var userEmail = entity.UserEmail.ToLower();
            var user = this._userRepository
                .GetData(x => x.UserName.ToLower() == userName || x.UserEmail.ToLower() == userEmail, null)
                .FirstOrDefault();

            if (user is not null)
            {
                return new ValidatorResult(false, Resources.ThereIsUser, nameof(UserFoundException), HandlesCode.BadRequest);
            }

            var result = await this.ValidateAsync(entity, cancellationToken);

            if (result.IsValid)
            {
                var acceptMessages = new List<IHandleMessage> 
                { 
                    HandleMessage.Factory("Id", entity.Id.ToString(), HandlesCode.Accepted),
                    HandleMessage.Factory("TenantId", entity.TenantId.ToString(), HandlesCode.Accepted),
                    HandleMessage.Factory("GroupId", entity.GroupId.ToString(), HandlesCode.Accepted),
                };

                return new ValidatorResult(true, acceptMessages);
            }

            var errorMessages = result.Errors.Select(x => HandleMessage.Factory(x.PropertyName, x.ErrorMessage, x.ErrorCode.ToEnum<HandlesCode>(HandlesCode.BadRequest)));
            return new ValidatorResult(false, errorMessages);
        }
    }
}
