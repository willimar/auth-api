using Account.Domain.Commands.Dtos;
using Account.Domain.Entities;
using Account.Domain.Extensions;
using AutoMapper;
using DataCore.Domain.Enumerators;
using DataCore.Mapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Mappers
{
    public class UserMapper : IMapperEntity
    {
        public void Mapper(IMapperConfigurationExpression profile)
        {
            SetAppendAccountMapper(profile);
            SetAppendUserMapper(profile);
        }

        private static void SetAppendAccountMapper(IMapperConfigurationExpression profile)
        {
            profile.CreateMap<AppendAccount, User>()
                .ForMember(dest => dest.FullName, map => map.MapFrom(source => source.FullName))
                .ForMember(dest => dest.UserEmail, map => map.MapFrom(source => source.UserEmail))
                .ForMember(dest => dest.UserName, map => map.MapFrom(source => source.UserName))
                .ForMember(dest => dest.TenantId, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(dest => dest.GroupId, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(dest => dest.Status, map => map.MapFrom(source => StatusRecord.Draft))
                .AfterMap(AppendUserAfterMapper)
                ;
        }

        private static void SetAppendUserMapper(IMapperConfigurationExpression profile)
        {
            profile.CreateMap<AppendUser, User>()
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
}
