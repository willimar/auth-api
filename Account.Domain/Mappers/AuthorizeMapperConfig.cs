using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Mappers
{
    public class AuthorizeMapperConfig : MapperConfiguration
    {
        public AuthorizeMapperConfig(AuthorizeMapper mapper)
            : base(mapper.Mapper)
        {
        }
    }
}
