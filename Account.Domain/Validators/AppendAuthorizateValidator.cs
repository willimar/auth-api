using Account.Domain.Entities;
using Account.Domain.Properties;
using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Validators
{
    public class AppendAuthorizateValidator : AbstractValidator<Authorize>, DataCore.Domain.Interfaces.IValidator<Authorize>
    {
        private readonly IRepositoryRead<Authorize> _repositoryRead;

        public AppendAuthorizateValidator(IRepositoryRead<Authorize> repositoryRead)
        {
            this._repositoryRead = repositoryRead;

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
            var name = entity.Name.ToLower();

            var authorize = this._repositoryRead
                .GetData(x => x.Name.ToLower() == name, null)
                .FirstOrDefault();

            if (authorize is not null)
            {
                return new ValidatorResult(false, Resources.ThereIsAuthorizate, nameof(RecordFoundException), HandlesCode.BadRequest);
            }

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
