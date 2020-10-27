using AutoMapper;
using city.core.entities;
using crud.api.core.fieldType;
using crud.api.core.mappers;
using crud.api.dto.Enums;
using crud.api.dto.Person;
using crud.api.register.entities.registers;
using crud.api.register.entities.registers.relational;
using Jwt.Simplify.Core.Entities;
using System;
using System.Collections.Generic;

namespace acount.api.Mappers
{
    public class PersonModelMapper : IMapperEntity 
    {
        public void Mapper(IMapperConfigurationExpression profile)
        {
            profile.CreateMap<PersonModel, Person<User>>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(src => src.PersonInfo.Name))
                .ForMember(dest => dest.NickName, map => map.MapFrom(src => src.PersonInfo.NickName))
                .ForMember(dest => dest.Birthday, map => map.MapFrom(src => src.PersonInfo.BirthDay))
                .ForMember(dest => dest.Gender, map => map.MapFrom(src => src.PersonInfo.Gender))
                .ForMember(dest => dest.MaritalStatus, map => map.MapFrom(src => src.PersonInfo.MaritalStatus))
                .ForMember(dest => dest.SpecialNeeds, map => map.MapFrom(src => src.PersonInfo.SpecialNeeds))
                .ForMember(dest => dest.Profession, map => map.MapFrom(src => src.PersonInfo.Profession))
                .ForMember(dest => dest.BirthCity, map => map.MapFrom(src => new City() { Id = Guid.NewGuid() }))
                .ForMember(dest => dest.RegisterDate, map => map.MapFrom(src => DateTime.UtcNow.AddMinutes(-3)))
                .ForMember(dest => dest.LastChangeDate, map => map.MapFrom(src => DateTime.UtcNow.AddMinutes(-3)))
                .ForMember(dest => dest.Status, map => map.MapFrom(src => RecordStatus.Active))
                .ForMember(dest => dest.User, map => map.MapFrom(src => new User() {
                    Email = src.UserInfo.UserEmail,
                    Login = src.UserInfo.UserName,
                    UserName = src.PersonInfo.Name,
                    Status = RecordStatus.Active,
                    RegisterDate = DateTime.UtcNow,
                    LastChangeDate = DateTime.UtcNow
                }))
                .ForMember(dest => dest.Contacts, map => map.MapFrom(src => GetContacts(src.PersonalContacts)))
                .ForMember(dest => dest.Addresses, map => map.MapFrom(src => GetAddress(src.Addresses)))
                .ForMember(dest => dest.Documents, map => map.MapFrom(src => GetDocuments(src.Documents)));
        }

        private IEnumerable<PersonDocument> GetDocuments(List<DictionaryFieldModel<DocumentType>> documents)
        {
            List<PersonDocument> result = new List<PersonDocument>();

            documents.ForEach(item => {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                result.Add(new PersonDocument() { Id = item.Id, LastChangeDate = DateTime.UtcNow, RegisterDate = DateTime.UtcNow, Status = RecordStatus.Active, Type = item.Type, Value = item.Value });
            });

            return result;
        }

        private IEnumerable<PersonAddress> GetAddress(List<AddressModel> addresses)
        {
            List<PersonAddress> result = new List<PersonAddress>();

            addresses.ForEach(item => {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                result.Add(new PersonAddress() { 
                    Id = item.Id, 
                    LastChangeDate = DateTime.UtcNow, 
                    RegisterDate = DateTime.UtcNow, 
                    Status = RecordStatus.Active,
                    City = string.Empty,
                    Complement = item.Complement,
                    
                });
            });

            return result;
        }

        private IEnumerable<PersonContact> GetContacts(List<DictionaryFieldModel<ContactType>> personalContacts)
        {
            List<PersonContact> result = new List<PersonContact>();

            personalContacts.ForEach(item => {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                result.Add(new PersonContact() { 
                    Id = item.Id,
                    LastChangeDate = DateTime.UtcNow,
                    RegisterDate = DateTime.UtcNow,
                    Status = RecordStatus.Active,
                    Type = item.Type,
                    Value = item.Value
                });
            });

            return result;
        }
    }
}
