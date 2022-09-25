using Account.Domain.Mappers;
using AutoMapper;

namespace Auvo.Financeiro.Application.Mappers.Fornecedor
{
    public class UserMapperConfig : MapperConfiguration
    {
        public UserMapperConfig(UserMapper mapper)
            : base(mapper.Mapper)
        {
        }
    }
}
