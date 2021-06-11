using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<DomainLogic.Entities.User, EFModels.User>()
                .ReverseMap();

            CreateMap<DomainLogic.Entities.SynchronizationProcess, EFModels.SynchronizationProcess>()
                .ReverseMap();

            CreateMap<DomainLogic.Entities.File, EFModels.File>()
                .ForMember(r => r.YandexResourceId, 
                            options => options.MapFrom(r => r.ResourceId))
                .ReverseMap()
                .ForMember(r => r.ResourceId, 
                                options => options.MapFrom(r => r.YandexResourceId));

            CreateMap<EFModels.IgnorePath, DomainLogic.Entities.IgnorePath>();
        }
    }
}
