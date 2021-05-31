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
        }
    }
}
