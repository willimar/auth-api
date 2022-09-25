using DataCore.Domain.Concrets;
using DataCore.Domain.Enumerators;
using DataCore.Domain.Interfaces;

namespace Account.Domain.Validators
{
    internal class ValidatorResult : IValidatorResult
    {
        public bool IsValid { get; private set; }

        public IEnumerable<IHandleMessage> Messages { get; set; } = new List<IHandleMessage>();

        public ValidatorResult(bool isValid, IEnumerable<IHandleMessage> messages)
        {
            IsValid = isValid;
            Messages = messages;
        }

        public ValidatorResult(bool isValid, string message, string name, HandlesCode handlesCode)
        {
            IsValid = isValid;

            Messages = new List<IHandleMessage>() { HandleMessage.Factory(name, message, handlesCode) };
        }

        public ValidatorResult(bool isValid = true)
        {
            IsValid = isValid;
        }

        private ValidatorResult()
        {

        }
    }
}
