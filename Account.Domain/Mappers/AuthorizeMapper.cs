using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using AutoMapper;
using DataCore.Domain.Enumerators;
using DataCore.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Mappers
{
    public class AuthorizeMapper : IMapperEntity
    {
        public void Mapper(IMapperConfigurationExpression profile)
        {
            SetAppendAuthorizeMapper(profile);
            SetChangeAuthorizeMapper(profile);
        }

        private void SetAppendAuthorizeMapper(IMapperConfigurationExpression profile)
        {
            profile.CreateMap<AppendAuthorize, Authorize>()
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name.ToLower()))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Description))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Active))
                ;
        }

        private void SetChangeAuthorizeMapper(IMapperConfigurationExpression profile)
        {
            profile.CreateMap<ChangeAuthorize, Authorize>()
                .ForMember(dest => dest.Name, map => map.Ignore())
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Description))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Active))
                .ForMember(dest => dest.Modified, map => map.Ignore())
                .ForMember(dest => dest.Created, map => map.Ignore())
                .ForMember(dest => dest.Id, map => map.Ignore())
                ;
        }
    }
}
