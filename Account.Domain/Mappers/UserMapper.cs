using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using Account.Domain.Extensions;
using AutoMapper;
using DataCore.Domain.Enumerators;
using DataCore.Mapper;

namespace Account.Domain.Mappers
{
    public class AppendAccountAppendUserMapper : MapperProfile<AppendAccount, AppendUser>
    {
        protected override void ForMemberMapper(IMappingExpression<AppendAccount, AppendUser> mapping)
        {
            _ = mapping
                .ForMember(dest => dest.FullName, map => map.MapFrom(source => source.FullName))
                .ForMember(dest => dest.UserEmail, map => map.MapFrom(source => source.UserEmail))
                .ForMember(dest => dest.UserName, map => map.MapFrom(source => source.UserName))
                ;
        }
    }

    public class AppendUserMapper : MapperProfile<AppendUser, User>
    {
        protected override void ForMemberMapper(IMappingExpression<AppendUser, User> mapping)
        {
            _ = mapping
                .ForMember(dest => dest.FullName, map => map.MapFrom(source => source.FullName))
                .ForMember(dest => dest.UserEmail, map => map.MapFrom(source => source.UserEmail))
                .ForMember(dest => dest.UserName, map => map.MapFrom(source => source.UserName))
                .ForMember(dest => dest.TenantId, map => map.MapFrom(source => source.TenantId))
                .ForMember(dest => dest.GroupId, map => map.MapFrom(source => source.GroupId))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Draft))
                .AfterMap(AppendUserAfterMapper)
                ;
        }

        private static void AppendUserAfterMapper(AppendAccount source, User dest)
        {
            var hashList = dest.GetHashTo(source.Password);

            hashList.ToList().ForEach(item => dest.UserHashes.Add(new UserHash { Value = item }));
        }
    }

    public class AppendAccountMapper : MapperProfile<AppendAccount, User>
    {
        protected override void ForMemberMapper(IMappingExpression<AppendAccount, User> mapping)
        {
            _ = mapping
                .ForMember(dest => dest.FullName, map => map.MapFrom(source => source.FullName))
                .ForMember(dest => dest.UserEmail, map => map.MapFrom(source => source.UserEmail))
                .ForMember(dest => dest.UserName, map => map.MapFrom(source => source.UserName))
                .ForMember(dest => dest.TenantId, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(dest => dest.GroupId, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Draft))
                .AfterMap(AppendUserAfterMapper)
                ;
        }

        private static void AppendUserAfterMapper(AppendAccount source, User dest)
        {
            var hashList = dest.GetHashTo(source.Password);

            hashList.ToList().ForEach(item => dest.UserHashes.Add(new UserHash { Value = item }));
        }
    }
}
