using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Profiles
{
    public class AuthursProfile : Profile
    {
        public AuthursProfile()
        {
            // CreateMap<Entities.Authors, Models.AuthorsDto>();
            /* The below code is to ensure that the calculated fields are also mapped properly, since the fields in the source and destination are not the same*/
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            // .ForMember();
        }
    }
}
