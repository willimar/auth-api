using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using AutoMapper;
using DataCore.Domain.Enumerators;
using DataCore.Mapper;

namespace Account.Domain.Mappers
{
    public class AppendAuthorizeMapper : MapperProfile<AppendAuthorize, Authorize>
    {
        protected override void ForMemberMapper(IMappingExpression<AppendAuthorize, Authorize> mapping)
        {
            _ = mapping
                .ForMember(dest => dest.Name, map => map.MapFrom(source => source.Name.ToLower()))
                .ForMember(dest => dest.Description, map => map.MapFrom(source => source.Description))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Active))
                .ForMember(dest => dest.Modified, map => map.Ignore())
                .ForMember(dest => dest.Created, map => map.Ignore())
                .ForMember(dest => dest.Id, map => map.Ignore())
                ;
        }
    }

    public class ChangeAuthorizeMapper : MapperProfile<ChangeAuthorize, Authorize>
    {
        protected override void ForMemberMapper(IMappingExpression<ChangeAuthorize, Authorize> mapping)
        {
            _ = mapping
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
