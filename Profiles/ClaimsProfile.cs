using AutoMapper;
using EliteForce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EliteForce.Profiles
{
    public class ClaimsProfile : Profile
    {
        public ClaimsProfile()
        {
            CreateMap<Claim, ClaimsDto>();
            CreateMap<Claim, ClaimItemsDto>();
        }
    }
}
