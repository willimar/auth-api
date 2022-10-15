using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using DataCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Domain.Extensions;
using DataCore.Jwt.Interfaces;
using DataCore.Jwt.Extensions;

namespace Account.Domain.Commands
{
    public class AuthenticateCommand
    {
        private readonly IRepositoryRead<User> _repositoryUser;
        private readonly IJwtOptions _jwtOptions;

        public AuthenticateCommand(IRepositoryRead<User> repositoryUser, IJwtOptions jwtOptions)
        {
            this._repositoryUser = repositoryUser;
            this._jwtOptions = jwtOptions;
        }

        public async ValueTask<(string token, string refreshToken, bool isValid)>Authenticate(AutenticateUser autenticateUser)
        {
            (string token, string refreshToken, bool isValid) response = new();

            var users = this._repositoryUser.GetData(x => x.UserName == autenticateUser.Login || x.UserEmail == autenticateUser.Login).ToList();

            response.isValid = false;

            if (users != null)
            {
                if (!users.Any())
                {
                    response.isValid = false;
                }
                else if (users.Count() > 1)
                {
                    response.isValid = false;
                }
                else 
                {
                    var user = users.First();
                    var hashList = user.GetHashTo(autenticateUser.Password);

                    if (user.UserHashes.Any(h => hashList.Any(hl => hl == h.Value)))
                    {
                        response.isValid = true;
                        response.token = user.CreateToken(this._jwtOptions);
                        response.refreshToken = user.CreateRefreshToken(this._jwtOptions);
                        return await ValueTask.FromResult(response);
                    }
                }
            }

            return await ValueTask.FromResult(response);
        }

        public async ValueTask<(string token, string refreshToken, bool isValid)>
            Authenticate(string refreshToken)
        {
            (string token, string refreshToken, bool isValid) response = new();

            response.isValid = false;
            var refreshUser = refreshToken.GetUser(this._jwtOptions);

            if (refreshUser != null)
            {
                var user = this._repositoryUser.GetData(x => x.Id == refreshUser.Id).FirstOrDefault();

                if (user != null)
                {
                    response.isValid = true;
                    response.token = user.CreateToken(this._jwtOptions);
                    response.refreshToken = user.CreateRefreshToken(this._jwtOptions);
                    return await ValueTask.FromResult(response);
                }
            }

            return await ValueTask.FromResult(response);
        }
    }
}
