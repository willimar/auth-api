using DataCore.Domain.Interfaces;

namespace Account.Domain.Extensions
{
    public static class UserExtensions
    {
        public static string[] GetHashTo(this IUser user, string password)
        {
            var hashFull = GetHashTo(user.TenantId,
                user.GroupId,
                user.UserName,
                user.UserEmail,
                password);

            var hashEmail = GetHashTo(user.TenantId,
                user.GroupId,
                user.UserEmail,
                password);

            var hashUser = GetHashTo(user.TenantId,
                user.GroupId,
                user.UserName,
                password);

            List<string> userHashes = new()
            {
                hashFull,
                hashEmail,
                hashUser
            };

            return userHashes.ToArray();
        }

        private static string GetHashTo(params object[] parameter)
        {
            return new { parameter }.ComputeMd5Hash();
        }
    }
}
