using AutoMapper;
using DomainLogic.Entities;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ResourcesFileItem, File>()
                .ReverseMap();
        }
    }
}
