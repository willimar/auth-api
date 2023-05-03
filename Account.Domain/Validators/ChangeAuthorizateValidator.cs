using Account.Domain.Entities;
using Account.Domain.Properties;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using FluentValidation;

namespace Account.Domain.Validators
{
    public class ChangeAuthorizateValidator : AbstractValidator<Authorize>, DataCore.Domain.Interfaces.IValidator<Authorize>
    {
        public ChangeAuthorizateValidator()
        {
            MapperValidation();
        }

        private void MapperValidation()
        {
            RuleFor(f => f.Name)
                .NotEmpty().MinimumLength(8).MaximumLength(50).WithMessage(Resources.AuthorizatenameLimits).WithErrorCode(HandlesCode.BadRequest.ToString());
            RuleFor(f => f.Description)
                .NotEmpty().MinimumLength(8).MaximumLength(100).WithMessage(Resources.AuthorizateDescriptionLimits).WithErrorCode(HandlesCode.BadRequest.ToString());
        }

        public async ValueTask<IValidatorResult> Validate(Authorize entity, CancellationToken cancellationToken)
        {
            var result = await this.ValidateAsync(entity, cancellationToken);

            if (result.IsValid)
            {
                var acceptMessages = new List<IHandleMessage>
                {
                    HandleMessage.Factory("Id", entity.Id.ToString(), HandlesCode.Accepted),
                    HandleMessage.Factory("Name", entity.Name.ToString(), HandlesCode.Accepted),
                    HandleMessage.Factory("Description", entity.Description.ToString(), HandlesCode.Accepted),
                };

                return new ValidatorResult(true, acceptMessages);
            }

            var errorMessages = result.Errors.Select(x => HandleMessage.Factory(x.PropertyName, x.ErrorMessage, x.ErrorCode.ToEnum<HandlesCode>(HandlesCode.BadRequest)));
            return new ValidatorResult(false, errorMessages);
        }
    }
}
