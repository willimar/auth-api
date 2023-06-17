namespace Account.Domain.Commands.Dtos
{
    public class AutenticateUser
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
